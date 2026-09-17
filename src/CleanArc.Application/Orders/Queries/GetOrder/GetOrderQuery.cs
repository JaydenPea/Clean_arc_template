namespace CleanArc.Application.Orders.Queries.GetOrder;

using CleanArc.Domain.Common;

public record GetOrderQuery(Guid OrderId);

public record OrderDto(
    Guid Id,
    string CustomerId,
    decimal Total,
    string Status,
    DateTimeOffset CreatedAt);
