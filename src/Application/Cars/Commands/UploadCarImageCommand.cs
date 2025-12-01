using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Cars.Exceptions;
using Domain.Cars;
using LanguageExt;
using MediatR;

namespace Application.Cars.Commands;

public record UploadCarImagesCommand : IRequest<Either<CarException, Car>>
{
    public required Guid CarId { get; init; }
    public required IReadOnlyList<ImageFileDto> Images { get; init; }
}

public record ImageFileDto
{
    public required string OriginalName { get; init; }
    public required Stream FileStream { get; init; }
}

public class UploadCarImagesCommandHandler(
    ICarRepository carRepository,
    ICarImageRepository carImageRepository,
    IFileStorage fileStorage)
    : IRequestHandler<UploadCarImagesCommand, Either<CarException, Car>>
{
    public async Task<Either<CarException, Car>> Handle(
        UploadCarImagesCommand request,
        CancellationToken cancellationToken)
    {
        var carId = new CarId(request.CarId);
        var existingCar = await carRepository.GetByIdAsync(carId, cancellationToken);

        return await existingCar.MatchAsync(
            async car => 
            {
                try
                {
                    var images = new List<CarImage>();
                    
                    foreach (var imageDto in request.Images)
                    {
                        var image = CarImage.New(car.Id, imageDto.OriginalName);
                        images.Add(image);
                        await fileStorage.UploadAsync(imageDto.FileStream, image.GetFilePath(), cancellationToken);
                    }
                    
                    await carImageRepository.AddRangeAsync(images, cancellationToken);
                    
                    return (Either<CarException, Car>)car;
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