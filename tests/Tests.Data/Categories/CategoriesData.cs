using Domain.Categories;
using Domain.Cars;

namespace Tests.Data.Categories;

public static class CategoriesData
{
    public static Category FirstTestCategory()
        => Category.New(CategoryId.New(), "Electric");

    public static Category SecondTestCategory()
        => Category.New(CategoryId.New(), "SUV");

    public static Category ThirdTestCategory()
        => Category.New(CategoryId.New(), "Sports");

    public static CategoryCar FirstTestCategoryCar(CategoryId categoryId, CarId carId)
        => CategoryCar.New(categoryId, carId);
}
