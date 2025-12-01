using Domain.Cars;

namespace Domain.Categories;

public class CategoryCar
{
    public CategoryId CategoryId { get; }
    public Category? Category { get; private set; }

    public CarId CarId { get; }
    public Car? Car { get; private set; }

    private CategoryCar(CategoryId categoryId, CarId carId)
        => (CategoryId, CarId) = (categoryId, carId);

    public static CategoryCar New(CategoryId categoryId, CarId carId)
        => new(categoryId, carId);
}