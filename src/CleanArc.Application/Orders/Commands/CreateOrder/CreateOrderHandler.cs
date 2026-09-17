namespace CleanArc.Application.Orders.Commands.CreateOrder;

using CleanArc.Application.Common.Interfaces;
using CleanArc.Domain.Common;
using CleanArc.Domain.Entities;

public interface ICreateOrderHandler
{
    Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct = default);
}

public sealed class CreateOrderHandler(
    IAppDbContext db,
    TimeProvider clock) : ICreateOrderHandler
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var items = request.Items
            .Select(i => OrderItem.Create(i.ProductId, i.Quantity, i.UnitPrice))
            .ToList();

        var order = Order.Create(request.CustomerId, items, clock.GetUtcNow());

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);

        return Result.Success(order.Id);
    }
}
