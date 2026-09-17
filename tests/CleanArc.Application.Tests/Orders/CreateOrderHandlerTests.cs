namespace CleanArc.Application.Tests.Orders;

using CleanArc.Application.Common.Interfaces;
using CleanArc.Application.Orders.Commands.CreateOrder;
using CleanArc.Domain.Entities;
using Xunit;

public class CreateOrderHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesOrderSuccessfully()
    {
        // Arrange
        var mockDbContext = new MockAppDbContext();
        var handler = new CreateOrderHandler(mockDbContext, TimeProvider.System);

        var command = new CreateOrderCommand(
            "CUST-001",
            new List<OrderItemDto>
            {
                new("PROD-001", 2, 29.99m),
                new("PROD-002", 1, 49.99m)
            });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Fact]
    public async Task Handle_WithInvalidQuantity_ThrowsException()
    {
        // Arrange
        var mockDbContext = new MockAppDbContext();
        var handler = new CreateOrderHandler(mockDbContext, TimeProvider.System);

        var command = new CreateOrderCommand(
            "CUST-001",
            new List<OrderItemDto>
            {
                new("PROD-001", 0, 29.99m) // Invalid: quantity must be > 0
            });

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
