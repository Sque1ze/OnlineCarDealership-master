using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Tests.Integration;

public class BasicApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BasicApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AuthToken_ShouldReturn_AccessToken()
    {
        // Arrange
        var request = new { Username = "admin", Password = "Password123!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/token", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var jsonString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonString);
        Assert.True(doc.RootElement.TryGetProperty("access_token", out _));
    }

    [Fact]
    public async Task GetCars_WithoutToken_ShouldReturn_Unauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/cars");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}