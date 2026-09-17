namespace CleanArc.Api.Endpoints;

using CleanArc.Application.Orders.Commands.CreateOrder;
using CleanArc.Application.Orders.Queries.GetOrder;
using Microsoft.AspNetCore.Http.HttpResults;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/orders")
            .WithName("Orders")
            .WithOpenApi();

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithDescription("Create a new order");

        group.MapGet("/{id:guid}", GetOrder)
            .WithName("GetOrder")
            .WithDescription("Get order by ID");
    }

    private static async Task<Created<Guid>> CreateOrder(
        CreateOrderCommand command,
        ICreateOrderHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(command, ct);

        return result.IsSuccess
            ? TypedResults.Created($"/api/orders/{result.Value}", result.Value)
            : throw new InvalidOperationException(result.Error);
    }

    private static async Task<Ok<OrderDto>> GetOrder(
        Guid id,
        IGetOrderHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(new GetOrderQuery(id), ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value!)
            : throw new InvalidOperationException(result.Error);
    }
}
