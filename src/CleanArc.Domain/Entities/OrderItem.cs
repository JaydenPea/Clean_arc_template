namespace CleanArc.Domain.Entities;

public class OrderItem
{
    private OrderItem() { }

    public string ProductId { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public static OrderItem Create(string productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(unitPrice));

        return new OrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public decimal GetLineTotal() => Quantity * UnitPrice;
}
