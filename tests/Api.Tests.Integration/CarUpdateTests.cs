using System.Net;
using System.Net.Http.Json;
using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Tests.Integration;

public class CarUpdateTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CarUpdateTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateCar_WithoutToken_ShouldReturn_Unauthorized()
    {
        // Arrange
        var updateDto = new
        {
            id = Guid.NewGuid(),          
            name = "Test Car",
            description = "Test desc",
            price = 10000m,
            stockQuantity = 1,
            categoryIds = Array.Empty<Guid>()
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/cars", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
