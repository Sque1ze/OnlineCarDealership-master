using Domain.Cars;

namespace Application.Cars.Exceptions;

public abstract class CarException(CarId carId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public CarId CarId { get; } = carId;
}

public class CarAlreadyExistException(CarId carId) 
    : CarException(carId, $"Car already exists under id {carId}");

public class CarNotFoundException(CarId carId) 
    : CarException(carId, $"Car not found under id {carId}");

public class CarCategoriesNotFoundException(CarId carId) 
    : CarException(carId, $"One or more categories not found for car {carId}");

public class CarImageNotFoundException(CarId carId, CarImageId imageId)
    : CarException(carId, $"Image {imageId} not found for car {carId}");

public class UnhandledCarException(CarId carId, Exception? innerException = null)
    : CarException(carId, "Unexpected error occurred", innerException);