using Domain.Cars;
using Domain.Customers;
using Domain.Orders;

namespace Tests.Data.Orders;

public static class OrdersData
{
    public static Order FirstTestOrder(CustomerId customerId, List<OrderItem> items)
        => Order.New(OrderId.New(), customerId, items);

    public static OrderItem FirstTestOrderItem(OrderId orderId, CarId carId, int quantity, decimal price)
        => OrderItem.New(orderId, carId, quantity, price);
}
