using Application.Common.Interfaces.Repositories;
using Application.Cars.Exceptions;
using Domain.Categories;
using Domain.Cars;
using LanguageExt;
using MediatR;

namespace Application.Cars.Commands;

public record CreateCarCommand : IRequest<Either<CarException, Car>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int StockQuantity { get; init; }
    public required IReadOnlyList<Guid> Categories { get; init; }
}

public class CreateCarCommandHandler(
    ICarRepository carRepository,
    ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCarCommand, Either<CarException, Car>>
{
    public async Task<Either<CarException, Car>> Handle(
        CreateCarCommand request,
        CancellationToken cancellationToken)
    {
        var existingCar = await carRepository.GetByNameAsync(request.Name, cancellationToken);

        return await existingCar.MatchAsync(
            f => new CarAlreadyExistException(f.Id),
            async () => await CreateEntity(request, cancellationToken));
    }

    private async Task<Either<CarException, Car>> CreateEntity(
        CreateCarCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var carId = CarId.New();
            var categoryIds = request.Categories.Select(x => new CategoryId(x)).ToList();
            var categories = await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);

            if (categories.Count != categoryIds.Count)
            {
                return new CarCategoriesNotFoundException(carId);
            }

            var categoryCars = categories
                .Select(c => CategoryCar.New(c.Id, carId))
                .ToList();

            var car = await carRepository.AddAsync(
                Car.New(carId, request.Name, request.Description, request.Price, request.StockQuantity, categoryCars),
                cancellationToken);

            return car;
        }
        catch (Exception exception)
        {
            return new UnhandledCarException(CarId.Empty(), exception);
        }
    }
}