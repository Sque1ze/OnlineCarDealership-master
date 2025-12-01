using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Orders.Exceptions;
using Domain.Customers;
using Domain.Cars;
using Domain.Orders;
using LanguageExt;
using MediatR;

namespace Application.Orders.Commands;

public record CreateOrderCommand : IRequest<Either<OrderException, Order>>
{
    public required Guid CustomerId { get; init; }
    public required IReadOnlyList<OrderItemDto> Items { get; init; }
}

public record OrderItemDto
{
    public required Guid CarId { get; init; }
    public required int Quantity { get; init; }
}

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    ICarRepository carRepository,
    IApplicationDbContext dbContext) 
    : IRequestHandler<CreateOrderCommand, Either<OrderException, Order>>
{
    public async Task<Either<OrderException, Order>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.Items.Any())
        {
            return new OrderEmptyException(OrderId.Empty());
        }

        var customerId = new CustomerId(request.CustomerId);
        var customerOption = await customerRepository.GetByIdAsync(customerId, cancellationToken);

        return await customerOption.MatchAsync(
            c => CreateOrderWithTransaction(c.Id, request, cancellationToken),
            () => Task.FromResult<Either<OrderException, Order>>(
                new OrderCustomerNotFoundException(OrderId.Empty())));
    }

    private async Task<Either<OrderException, Order>> CreateOrderWithTransaction(
        CustomerId customerId,
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {

        using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);

        try
        {
            var carIds = request.Items
                .Select(i => new CarId(i.CarId))
                .Distinct()
                .ToList();

            var cars = await carRepository.GetByIdsAsync(carIds, cancellationToken);

            var carsMap = cars.ToDictionary(f => f.Id);

            if (cars.Count != carIds.Count)
            {
                return new OrderCarNotFoundException(OrderId.Empty()); 
            }

            var orderId = OrderId.New();
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in request.Items)
            {
                var carId = new CarId(itemDto.CarId);
                var car = carsMap[carId];
                
                if (car.StockQuantity < itemDto.Quantity)
                {
                    return new InsufficientStockForOrderException(
                        orderId,
                        carId.Value,
                        car.Name,
                        itemDto.Quantity,
                        car.StockQuantity);
                }

                car.DecreaseStock(itemDto.Quantity);

                orderItems.Add(OrderItem.New(orderId, carId, itemDto.Quantity, car.Price));
                
                await carRepository.UpdateAsync(car, cancellationToken);
            }

            var order = Order.New(orderId, customerId, orderItems);
            await orderRepository.AddAsync(order, cancellationToken);
            
            transaction.Commit();

            return order;
        }
        catch (Exception exception)
        {
            return new UnhandledOrderException(OrderId.Empty(), exception);
        }
    }
}