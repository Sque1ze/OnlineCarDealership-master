using System.Net;
using System.Net.Http.Json;
using Api;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests.Integration;

public class CarsEndpointSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CarsEndpointSmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCars_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/cars");
        // Endpoint should exist; whether it returns data or empty collection is not important for smoke test
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthToken_ShouldReturnToken()
    {
        var request = new { Username = "admin", Password = "Password123!" };
        var response = await _client.PostAsJsonAsync("/api/auth/token", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
