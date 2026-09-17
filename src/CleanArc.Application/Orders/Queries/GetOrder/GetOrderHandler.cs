namespace CleanArc.Application.Orders.Queries.GetOrder;

using CleanArc.Application.Common.Interfaces;
using CleanArc.Domain.Common;
using Microsoft.EntityFrameworkCore;

public interface IGetOrderHandler
{
    Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken ct = default);
}

public sealed class GetOrderHandler(IAppDbContext db) : IGetOrderHandler
{
    public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken ct)
    {
        var order = await db.Orders
            .Where(o => o.Id == request.OrderId)
            .Select(o => new OrderDto(
                o.Id,
                o.CustomerId,
                o.Total,
                o.Status.ToString(),
                o.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return order is not null
            ? Result.Success(order)
            : Result.Failure<OrderDto>("Order not found");
    }
}
