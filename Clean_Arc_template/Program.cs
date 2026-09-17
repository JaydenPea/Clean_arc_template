using CleanArc.Api.Extensions;
using CleanArc.Application.Orders.Commands.CreateOrder;
using CleanArc.Application.Orders.Queries.GetOrder;
using CleanArc.Infrastructure;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

// Register application handlers
builder.Services.AddScoped<ICreateOrderHandler, CreateOrderHandler>();
builder.Services.AddScoped<IGetOrderHandler, GetOrderHandler>();

// Register infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Auto-discover and map all endpoints
app.MapApiEndpoints();

app.Run();
