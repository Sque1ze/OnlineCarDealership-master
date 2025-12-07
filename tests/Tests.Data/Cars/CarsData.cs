using Domain.Cars;
using Domain.Categories;

namespace Tests.Data.Cars;

public static class CarsData
{
    public static Car FirstTestCar()
        => Car.New(
            CarId.New(),
            "Tesla Model 3",
            "Compact electric sedan",
            35000m,
            5,
            new List<CategoryCar>());

    public static Car SecondTestCar()
        => Car.New(
            CarId.New(),
            "BMW X5",
            "Luxury SUV",
            65000m,
            2,
            new List<CategoryCar>());

    public static CarImage FirstTestCarImage(CarId carId)
        => CarImage.New(carId, "test-image-1.jpg");

    public static CarImage SecondTestCarImage(CarId carId)
        => CarImage.New(carId, "test-image-2.jpg");
}
