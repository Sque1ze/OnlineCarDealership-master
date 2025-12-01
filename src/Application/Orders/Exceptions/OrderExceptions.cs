using Domain.Orders;

namespace Application.Orders.Exceptions;

public abstract class OrderException(OrderId orderId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public OrderId OrderId { get; } = orderId;
}

public class OrderNotFoundException(OrderId orderId) 
    : OrderException(orderId, $"Order not found under id {orderId}");

public class OrderCustomerNotFoundException(OrderId orderId)
    : OrderException(orderId, $"Customer not found for order {orderId}");

public class OrderCarNotFoundException(OrderId orderId)
    : OrderException(orderId, $"One or more cars not found for order {orderId}");

public class OrderEmptyException(OrderId orderId)
    : OrderException(orderId, $"Order {orderId} cannot be empty");

public class InsufficientStockForOrderException(
    OrderId orderId, 
    Guid carId, 
    string carName, 
    int requested, 
    int available)
    : OrderException(
        orderId, 
        $"Insufficient stock for car '{carName}' (ID: {carId}). Requested: {requested}, Available: {available}");

public class UnhandledOrderException(OrderId orderId, Exception? innerException = null)
    : OrderException(orderId, "Unexpected error occurred", innerException);