using Api.Dtos;
using Domain.Cars;
using Domain.Categories;
using FluentAssertions;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Tests.Common;
using Tests.Data.Cars;
using Tests.Data.Categories;
using Xunit;

namespace Api.Tests.Integration.Cars;

public class CarsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private const string BaseRoute = "/api/cars";

    private readonly Category _category = CategoriesData.FirstTestCategory();
    private readonly Car _existingCar = CarsData.FirstTestCar();

    public CarsControllerTests(IntegrationTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ShouldGetAllCars()
    {
        // Act
        var response = await Client.GetAsync(BaseRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cars = await response.ToResponseModel<List<CarDto>>();
        cars.Should().HaveCount(1);
        cars.First().Name.Should().Be(_existingCar.Name);
    }

    [Fact]
    public async Task ShouldCreateCar()
    {
        // Arrange
        var request = new CreateCarDto(
            "New Car",
            "Test description",
            12345m,
            10,
            new[] { _category.Id.Value });

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.ToResponseModel<CarDto>();
        dto.Id.Should().NotBe(Guid.Empty);
        dto.Name.Should().Be("New Car");

        var dbCar = await Context.Cars
            .Include(c => c.Categories)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Value == dto.Id);

        dbCar.Should().NotBeNull();
        dbCar!.Categories.Should().NotBeNull();
        dbCar.Categories!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldNotCreateCar_WhenNameIsEmpty()
    {
        var request = new CreateCarDto(
            "",
            "Desc",
            1000m,
            1,
            new[] { _category.Id.Value });

        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldDeleteCar()
    {
        var route = $"{BaseRoute}/{_existingCar.Id.Value}";

        var response = await Client.DeleteAsync(route);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var exists = await Context.Cars.AnyAsync(x => x.Id == _existingCar.Id);
        exists.Should().BeFalse();
    }

    public async Task InitializeAsync()
    {
        await Context.Categories.AddAsync(_category);
        await Context.Cars.AddAsync(_existingCar);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.CarImages.RemoveRange(Context.CarImages);
        Context.Cars.RemoveRange(Context.Cars);
        Context.Categories.RemoveRange(Context.Categories);
        await SaveChangesAsync();
    }
}
