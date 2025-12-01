using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Cars.Exceptions;
using Domain.Cars;
using LanguageExt;
using MediatR;

namespace Application.Cars.Commands;

public record DeleteCarImageCommand : IRequest<Either<CarException, Car>>
{
    public required Guid CarId { get; init; }
    public required Guid ImageId { get; init; }
}

public class DeleteCarImageCommandHandler(
    ICarRepository carRepository,
    ICarImageRepository carImageRepository,
    IFileStorage fileStorage)
    : IRequestHandler<DeleteCarImageCommand, Either<CarException, Car>>
{
    public async Task<Either<CarException, Car>> Handle(
        DeleteCarImageCommand request,
        CancellationToken cancellationToken)
    {
        var carId = new CarId(request.CarId);
        var imageId = new CarImageId(request.ImageId);
        
        var existingCar = await carRepository.GetByIdAsync(carId, cancellationToken);

        return await existingCar.MatchAsync(
            async car => 
            {
                try
                {
                    var imageOption = await carImageRepository.GetByIdAsync(imageId, cancellationToken);
                    
                    return await imageOption.MatchAsync(
                        async image =>
                        {
                            if (image.CarId != car.Id)
                            {
                                return (Either<CarException, Car>)new CarImageNotFoundException(carId, imageId);
                            }

                            await fileStorage.DeleteAsync(image.GetFilePath(), cancellationToken);
                            
                            await carImageRepository.DeleteAsync(image, cancellationToken);
                            
                            car.RemoveImage(imageId);
                            await carRepository.UpdateAsync(car, cancellationToken);
                            
                            return (Either<CarException, Car>)car;
                        },
                        () => Task.FromResult<Either<CarException, Car>>(
                            new CarImageNotFoundException(carId, imageId)));
                }
                catch (Exception exception)
                {
                    return (Either<CarException, Car>)new UnhandledCarException(car.Id, exception);
                }
            },
            () => Task.FromResult<Either<CarException, Car>>(
                new CarNotFoundException(carId)));
    }
}