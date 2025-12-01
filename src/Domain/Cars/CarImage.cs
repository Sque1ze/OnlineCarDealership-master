namespace Domain.Cars;

public class CarImage
{
    public CarImageId Id { get; }
    public string OriginalName { get; }
    public CarId CarId { get; }

    private CarImage(CarImageId id, string originalName, CarId carId)
    {
        Id = id;
        OriginalName = originalName;
        CarId = carId;
    }

    public static CarImage New(CarId carId, string originalName)
    {
        return new CarImage(CarImageId.New(), originalName, carId);
    }

    public string GetFilePath()
        => $"{CarId}/{Id}{Path.GetExtension(OriginalName)}";
}