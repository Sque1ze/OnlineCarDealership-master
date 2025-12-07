using Api.Dtos;
using Domain.Cars;
using Domain.Customers;
using Domain.Orders;
using FluentAssertions;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Tests.Common;
using Tests.Data.Cars;
using Tests.Data.Customers;
using Tests.Data.Orders;
using Xunit;

namespace Api.Tests.Integration.Orders;

public class OrdersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private const string BaseRoute = "/api/orders";

    private readonly Customer _customer = CustomersData.FirstTestCustomer();
    private readonly Car _car = CarsData.FirstTestCar();

    public OrdersControllerTests(IntegrationTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact(Skip = "TODO: Fix after Orders API logic is clarified")]
    public async Task ShouldCreateOrder()
    {
        var items = new[]
        {
            new CreateOrderItemDto(_car.Id.Value, 2)
        };

        var request = new CreateOrderDto(_customer.Id.Value, items);

        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var dto = await response.ToResponseModel<OrderDto>();
        dto.CustomerId.Should().Be(_customer.Id.Value);
        dto.Items.Should().HaveCount(1);
        dto.Items.First().Quantity.Should().Be(2);

        var dbOrder = await Context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Value == dto.Id);

        dbOrder.Should().NotBeNull();
        dbOrder!.Items.Should().HaveCount(1);
    }

    public async Task InitializeAsync()
    {
        await Context.Customers.AddAsync(_customer);
        await Context.Cars.AddAsync(_car);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.OrderItems.RemoveRange(Context.OrderItems);
        Context.Orders.RemoveRange(Context.Orders);
        Context.Cars.RemoveRange(Context.Cars);
        Context.Customers.RemoveRange(Context.Customers);
        await SaveChangesAsync();
    }
}
