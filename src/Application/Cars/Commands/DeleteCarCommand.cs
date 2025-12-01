using Application.Common.Interfaces.Repositories;
using Application.Cars.Exceptions;
using Domain.Cars;
using LanguageExt;
using MediatR;

namespace Application.Cars.Commands;

public record DeleteCarCommand : IRequest<Either<CarException, Car>>
{
    public required Guid CarId { get; init; }
}

public class DeleteCarCommandHandler(ICarRepository carRepository)
    : IRequestHandler<DeleteCarCommand, Either<CarException, Car>>
{
    public async Task<Either<CarException, Car>> Handle(
        DeleteCarCommand request,
        CancellationToken cancellationToken)
    {
        var carId = new CarId(request.CarId);
        var existingCar = await carRepository.GetByIdAsync(carId, cancellationToken);

        return await existingCar.MatchAsync(
            f => DeleteEntity(f, cancellationToken),
            () => Task.FromResult<Either<CarException, Car>>(
                new CarNotFoundException(carId)));
    }

    private async Task<Either<CarException, Car>> DeleteEntity(
        Car car,
        CancellationToken cancellationToken)
    {
        try
        {
            return await carRepository.DeleteAsync(car, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UnhandledCarException(car.Id, exception);
        }
    }
}