using Api.Dtos;
using Domain.Customers;
using FluentAssertions;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Tests.Common;
using Tests.Data.Customers;
using Xunit;

namespace Api.Tests.Integration.Customers;

public class CustomersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private const string BaseRoute = "/api/customers";

    private readonly Customer _testCustomer = CustomersData.FirstTestCustomer();

    private string DetailRoute => $"{BaseRoute}/{_testCustomer.Id.Value}";

    public CustomersControllerTests(IntegrationTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ShouldGetAllCustomers()
    {
        var response = await Client.GetAsync(BaseRoute);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var customers = await response.ToResponseModel<List<CustomerDto>>();
        customers.Should().NotBeEmpty();
        customers.Should().Contain(c => c.Email == _testCustomer.Email);
    }

    [Fact]
    public async Task ShouldCreateCustomer()
    {
        var request = new CreateCustomerDto(
            "New",
            "User",
            "new.user@example.com",
            "+380501112233",
            "Some address");

        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var dto = await response.ToResponseModel<CustomerDto>();
        dto.Email.Should().Be("new.user@example.com");

        var dbCustomer = await Context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Value == dto.Id);

        dbCustomer.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldDeleteCustomer()
    {
        // ARRANGE
        var createRequest = new CreateCustomerDto(
            "ToDelete",
            "User",
            "delete.user@example.com",
            "+380501112233",
            "Some address");

        var createResponse = await Client.PostAsJsonAsync(BaseRoute, createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdCustomer = await createResponse.ToResponseModel<CustomerDto>();
        var deleteRoute = $"{BaseRoute}/{createdCustomer.Id}";

        // ACT
        var deleteResponse = await Client.DeleteAsync(deleteRoute);

        // ASSERT
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await Client.GetAsync(deleteRoute);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    public async Task InitializeAsync()
    {
        var request = new CreateCustomerDto(
            _testCustomer.FirstName,
            _testCustomer.LastName,
            _testCustomer.Email,
            _testCustomer.Phone,
            _testCustomer.Address);

        var response = await Client.PostAsJsonAsync(BaseRoute, request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DisposeAsync()
    {
        Context.Customers.RemoveRange(Context.Customers);
        await SaveChangesAsync();
    }
}
