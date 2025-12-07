using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Tests.Integration.Categories;

public class CategoriesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoriesControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var request = new { Username = "admin", Password = "Password123!" };

        var response = await _client.PostAsJsonAsync("/api/auth/token", request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("access_token").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetCategories_ShouldReturnOk()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json",
            response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnCreated_AndContainName()
    {
        // Arrange
        await AuthenticateAsync();

        var dto = new { name = $"Electric_{Guid.NewGuid()}" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/categories", dto);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.OK,
            $"Expected 201 or 200, got {response.StatusCode}");

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        var returnedName = json.GetProperty("name").GetString();
        Assert.Equal(dto.name, returnedName);

        var id = json.GetProperty("id").GetGuid();
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task CreateCategory_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        await AuthenticateAsync();

        var dto = new { name = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/categories", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCategory_ByNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        await AuthenticateAsync();

        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/categories/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_WithInvalidGuid_ShouldReturnNotFound()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.DeleteAsync("/api/categories/not-a-guid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}