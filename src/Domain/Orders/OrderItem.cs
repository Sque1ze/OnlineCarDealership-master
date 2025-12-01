using Domain.Cars;

namespace Domain.Orders;

public class OrderItem
{
    public OrderItemId Id { get; }
    public OrderId OrderId { get; }
    public Order? Order { get; private set; }
    
    public CarId CarId { get; }
    public Car? Car { get; private set; }
    
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    private OrderItem(OrderItemId id, OrderId orderId, CarId carId, int quantity, decimal price)
    {
        Id = id;
        OrderId = orderId;
        CarId = carId;
        Quantity = quantity;
        Price = price;
    }

    public static OrderItem New(OrderId orderId, CarId carId, int quantity, decimal price)
        => new(OrderItemId.New(), orderId, carId, quantity, price);
}