using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Cars.Exceptions;
using Domain.Categories;
using Domain.Cars;
using LanguageExt;
using MediatR;

namespace Application.Cars.Commands;

public record UpdateCarCommand : IRequest<Either<CarException, Car>>
{
    public required Guid CarId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int StockQuantity { get; init; }
    public required IReadOnlyList<Guid> Categories { get; init; }
}

public class UpdateCarCommandHandler(
    ICarRepository carRepository,
    ICategoryRepository categoryRepository,
    ICategoryCarRepository categoryCarRepository,
    IApplicationDbContext dbContext) 
    : IRequestHandler<UpdateCarCommand, Either<CarException, Car>>
{
    public async Task<Either<CarException, Car>> Handle(
        UpdateCarCommand request,
        CancellationToken cancellationToken)
    {
        var carId = new CarId(request.CarId);
        var existingCar = await carRepository.GetByIdAsync(carId, cancellationToken);

        return await existingCar.MatchAsync(
            f => UpdateEntity(f, request, cancellationToken),
            () => Task.FromResult<Either<CarException, Car>>(
                new CarNotFoundException(carId)));
    }

    private async Task<Either<CarException, Car>> UpdateEntity(
        Car car,
        UpdateCarCommand request,
        CancellationToken cancellationToken)
    {
        using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);

        try
        {
            var categoryIds = request.Categories.Select(x => new CategoryId(x)).ToList();
            var categories = await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);

            if (categories.Count != categoryIds.Count)
            {
                return new CarCategoriesNotFoundException(car.Id);
            }
            
            car.UpdateDetails(request.Name, request.Description, request.Price, request.StockQuantity);


            var existingCategories = await categoryCarRepository.GetByCarIdAsync(car.Id, cancellationToken);
            await categoryCarRepository.RemoveRangeAsync(existingCategories, cancellationToken);
            
            var newCategoryCars = categories
                .Select(c => CategoryCar.New(c.Id, car.Id))
                .ToList();

            await categoryCarRepository.AddRangeAsync(newCategoryCars, cancellationToken);
            await carRepository.UpdateAsync(car, cancellationToken);
            transaction.Commit();

            return car;
        }
        catch (Exception exception)
        {
            return new UnhandledCarException(car.Id, exception);
        }
    }
}