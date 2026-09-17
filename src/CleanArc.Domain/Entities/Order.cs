namespace CleanArc.Domain.Entities;

using CleanArc.Domain.Common;
using CleanArc.Domain.Enums;

public class Order : Entity
{
    private readonly List<OrderItem> _items = [];

    private Order() { }

    public string CustomerId { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(string customerId, IEnumerable<OrderItem> items, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            CreatedAt = now
        };

        foreach (var item in items)
            order.AddItem(item);

        if (!order._items.Any())
            throw new ArgumentException("Order must have at least one item", nameof(items));

        return order;
    }

    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending order");

        _items.Add(item);
        RecalculateTotal();
    }

    public Result Cancel()
    {
        if (Status is not OrderStatus.Pending)
            return Result.Failure("Only pending orders can be cancelled");

        Status = OrderStatus.Cancelled;
        return Result.Success();
    }

    public Result Confirm()
    {
        if (Status is not OrderStatus.Pending)
            return Result.Failure("Only pending orders can be confirmed");

        Status = OrderStatus.Confirmed;
        return Result.Success();
    }

    private void RecalculateTotal()
    {
        Total = _items.Sum(i => i.GetLineTotal());
    }
}
