using Api.Dtos;
using Domain.Categories;
using FluentAssertions;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Tests.Common;
using Tests.Data.Categories;
using Xunit;

namespace Api.Tests.Integration.Categories;

public class CategoriesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private const string BaseRoute = "/api/categories";

    private readonly Category _firstTestCategory = CategoriesData.FirstTestCategory();
    private readonly Category _secondTestCategory = CategoriesData.SecondTestCategory();

    private string DetailRoute => $"{BaseRoute}/{_firstTestCategory.Id.Value}";

    public CategoriesControllerTests(IntegrationTestWebFactory factory)
        : base(factory)
    {
    }

    // ---------- GET ----------

    [Fact]
    public async Task ShouldGetAllCategories()
    {
        // Act
        var response = await Client.GetAsync(BaseRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categories = await response.ToResponseModel<List<CategoryDto>>();

        categories.Should().NotBeEmpty();
        categories.Should().Contain(c => c.Name == _firstTestCategory.Name);

        var category = categories.First();
        category.Id.Should().Be(_firstTestCategory.Id.Value);
        category.Name.Should().Be(_firstTestCategory.Name);
    }

    [Fact]
    public async Task ShouldGetCategoryById()
    {
        // Act
        var response = await Client.GetAsync(DetailRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.ToResponseModel<CategoryDto>();
        dto.Id.Should().Be(_firstTestCategory.Id.Value);
        dto.Name.Should().Be(_firstTestCategory.Name);
        dto.CreatedAt.Should().BeCloseTo(_firstTestCategory.CreatedAt, TimeSpan.FromSeconds(2));
        dto.UpdatedAt.Should().Be(_firstTestCategory.UpdatedAt);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var route = $"{BaseRoute}/{Guid.NewGuid()}";

        // Act
        var response = await Client.GetAsync(route);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- POST ----------

    [Fact]
    public async Task ShouldCreateCategory()
    {
        // Arrange
        var request = new CreateCategoryDto("Hatchback");

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var dto = await response.ToResponseModel<CategoryDto>();
        dto.Id.Should().NotBe(Guid.Empty);
        dto.Name.Should().Be("Hatchback");

        var dbCategory = await Context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Value == dto.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be("Hatchback");
    }

    [Fact]
    public async Task ShouldNotCreateCategory_WithEmptyName()
    {
        // Arrange
        var request = new CreateCategoryDto("");

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldNotCreateCategory_WithTooLongName()
    {
        // Arrange
        var longName = new string('a', 256); // > 255
        var request = new CreateCategoryDto(longName);

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldCreateCategory_WithMaxAllowedNameLength()
    {
        // Arrange
        var maxName = new string('a', 255);
        var request = new CreateCategoryDto(maxName);

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var dto = await response.ToResponseModel<CategoryDto>();
        dto.Name.Should().Be(maxName);
    }

    // ---------- PUT ----------

    [Fact]
    public async Task ShouldUpdateCategory()
    {
        // Arrange
        var request = new UpdateCategoryDto("Updated category name");

        // Act
        var response = await Client.PutAsJsonAsync(DetailRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.ToResponseModel<CategoryDto>();
        dto.Name.Should().Be("Updated category name");
        dto.UpdatedAt.Should().NotBeNull();

        var dbCategory = await Context.Categories
            .AsNoTracking()
            .FirstAsync(x => x.Id == _firstTestCategory.Id);

        dbCategory.Name.Should().Be("Updated category name");
        dbCategory.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenUpdatingNonExistingCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var route = $"{BaseRoute}/{id}";
        var request = new UpdateCategoryDto("Name");

        // Act
        var response = await Client.PutAsJsonAsync(route, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateCategory_WithEmptyName()
    {
        // Arrange
        var request = new UpdateCategoryDto("");

        // Act
        var response = await Client.PutAsJsonAsync(DetailRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldNotUpdateCategory_WithTooLongName()
    {
        // Arrange
        var longName = new string('a', 256);
        var request = new UpdateCategoryDto(longName);

        // Act
        var response = await Client.PutAsJsonAsync(DetailRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldNotUpdateCategory_WithExistingName()
    {
        // Arrange
        await Context.Categories.AddAsync(_secondTestCategory);
        await SaveChangesAsync();

        var request = new UpdateCategoryDto(_secondTestCategory.Name);

        // Act
        var response = await Client.PutAsJsonAsync(DetailRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbCategory = await Context.Categories.FirstAsync(x => x.Id == _firstTestCategory.Id);
        dbCategory.Name.Should().Be(_firstTestCategory.Name);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task ShouldDeleteCategory()
    {
        // Act
        var response = await Client.DeleteAsync(DetailRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var exists = await Context.Categories
            .AnyAsync(x => x.Id == _firstTestCategory.Id);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenDeletingNonExistingCategory()
    {
        // Arrange
        var route = $"{BaseRoute}/{Guid.NewGuid()}";

        // Act
        var response = await Client.DeleteAsync(route);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Categories.AddAsync(_firstTestCategory);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Categories.RemoveRange(Context.Categories);
        await SaveChangesAsync();
    }
}
