namespace CleanArc.Application.Orders.Commands.CreateOrder;

using CleanArc.Domain.Common;

public record OrderItemDto(string ProductId, int Quantity, decimal UnitPrice);

public record CreateOrderCommand(
    string CustomerId,
    List<OrderItemDto> Items);
