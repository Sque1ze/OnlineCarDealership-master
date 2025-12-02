using System.Collections.Generic;
using Domain.Cars;
using Domain.Categories;
using Xunit;

namespace Tests.Data;

public class CarCreationTests
{
    [Fact]
    public void NewCar_ShouldCreateValidCar_WithBasicData()
    {
        // Arrange
        var id = CarId.New();
        var name = "Tesla Model S";
        decimal price = 120000m;
        int stock = 10;

        var categories = new List<CategoryCar>();

        // Act
        var car = Car.New(
            id,
            name,
            null,
            price,
            stock,
            categories);

        // Assert
        Assert.NotNull(car);
        Assert.Equal(id.Value, car.Id);
        Assert.Equal(name, car.Name);
        Assert.Equal(price, car.Price);
        Assert.Equal(stock, car.StockQuantity);
    }
}
