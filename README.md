# Clean Architecture Template for .NET 10

A production-ready Clean Architecture template demonstrating proper dependency inversion, layer isolation, and architectural constraints enforcement. This project serves as a blueprint for scalable .NET applications.

## 🏗️ Architecture Overview

This project implements **Clean Architecture** principles with strict dependency inversion. The architecture consists of five core projects:

```
CleanArc.Domain/          ← Core business logic (no external dependencies)
├── Entities/
├── Enums/
└── Interfaces/

CleanArc.Application/     ← Use cases and orchestration
├── Orders/Commands/
├── Orders/Queries/
└── Validators/

CleanArc.Infrastructure/  ← Persistence and external services
├── Persistence/
└── DependencyInjection.cs

CleanArc.Api/             ← HTTP API layer (Minimal APIs)
├── Endpoints/
├── Extensions/
└── Program.cs

CleanArc.Application.Tests/ ← Architecture & behavior tests
├── ArchitectureTests.cs
└── *Handler Tests
```

### Dependency Direction

All dependencies flow **inward** toward the Domain layer:

```
Infrastructure ──┐
                 ├──→ Application ──→ Domain
Api ─────────────┘
                 (Tests verify this)
```

**Key Principle:** The Domain layer has **zero** external dependencies. It contains pure business logic that can be tested in isolation.

## 🎯 Core Features

### 1. **Dependency Inversion via Interfaces**

The Application layer defines interfaces (`IAppDbContext`), while Infrastructure implements them. This allows:
- Testing with in-memory implementations
- Swapping persistence mechanisms without changing use cases
- Complete isolation of business logic from infrastructure concerns

```csharp
// Application layer (defines interface)
public interface IAppDbContext
{
    DbSet<Order> Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}

// Infrastructure layer (implements)
public class AppDbContext : DbContext, IAppDbContext
{
    // EF Core implementation
}

// Handler uses interface
public class CreateOrderHandler : ICreateOrderHandler
{
    private readonly IAppDbContext _context;
    // Business logic remains independent of EF Core
}
```

### 2. **CQRS-Inspired Command/Query Handlers**

Commands (write operations) and Queries (read operations) are separated into dedicated handlers:

```
Command → Validation → Handler → Persist → Result<T>
Query   → Handler → Retrieve → Result<T>
```

Each handler:
- Validates input using FluentValidation
- Encapsulates a single use case
- Returns `Result<T>` (success/failure pattern)
- Is dependency-injected as `IHandler`

### 3. **Auto-Discovery Endpoint Pattern**

Endpoints are discovered automatically via reflection. No manual wiring in `Program.cs`:

```csharp
// Program.cs - ONE LINE for ALL endpoints
app.MapApiEndpoints();  // Auto-discovers Map* methods

// Endpoints/OrderEndpoints.cs
public static class OrderEndpoints
{
    public static void MapOrderEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/orders");
        group.MapPost("/", CreateOrder);
        group.MapGet("/{id:guid}", GetOrder);
    }
}
```

**Why?** Follows the Open/Closed Principle. Add new endpoints without modifying `Program.cs`.

### 4. **Architecture Constraint Testing**

The test project enforces architectural rules at compile time using **NetArchTest.Rules**:

```csharp
[Fact]
public void Domain_Should_Not_Depend_On_Application()
{
    var result = Types.InAssembly(_domainAssembly)
        .That()
        .ResideInNamespace("CleanArc.Domain")
        .ShouldNot()
        .HaveDependencyOn("CleanArc.Application")
        .GetResult();

    Assert.True(result.IsSuccessful);
}
```

These tests prevent architectural drift as the codebase grows.

### 5. **Result<T> Pattern for Explicit Error Handling**

Instead of exceptions for business logic failures, use `Result<T>`:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

Benefits:
- Explicit success/failure paths
- No hidden exceptions for business logic
- Type-safe error information
- Integrates cleanly with HTTP responses

### 6. **FluentValidation Integration**

Validators are auto-registered from the Application assembly:

```csharp
// Register all validators
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

// Define validators
public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must have at least one item");
    }
}
```

Validation occurs in handlers before business logic executes.

### 7. **Entity Framework Core with PostgreSQL**

The Infrastructure layer abstracts persistence:

```csharp
public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
```

Configuration via Fluent API keeps database mappings organized and testable.

## 📁 Project Structure

```
src/
├── CleanArc.Domain/
│   ├── Entities/
│   │   ├── Order.cs              # Aggregate root with invariants
│   │   └── OrderItem.cs          # Value object
│   └── Enums/
│       └── OrderStatus.cs        # Domain enum

├── CleanArc.Application/
│   ├── Orders/
│   │   ├── Commands/
│   │   │   └── CreateOrder/
│   │   │       ├── CreateOrderCommand.cs
│   │   │       ├── CreateOrderHandler.cs
│   │   │       └── CreateOrderValidator.cs
│   │   └── Queries/
│   │       └── GetOrder/
│   │           ├── GetOrderQuery.cs
│   │           └── GetOrderHandler.cs
│   └── Interfaces/
│       └── IAppDbContext.cs      # Persistence abstraction

├── CleanArc.Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   └── OrderConfiguration.cs
│   └── DependencyInjection.cs    # Service registration

├── Clean_Arc_template/            # API Project
│   ├── Endpoints/
│   │   └── OrderEndpoints.cs
│   ├── Extensions/
│   │   └── EndpointExtensions.cs # Auto-discovery
│   └── Program.cs

tests/
└── CleanArc.Application.Tests/
    ├── ArchitectureTests.cs      # Enforce layer boundaries
    └── *Handler.Tests.cs         # Behavior tests
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 12+ (or configure SQL Server/SQLite in `appsettings.json`)
- Visual Studio 2022, VS Code, or JetBrains Rider

### Setup

1. **Clone and restore:**
   ```bash
   git clone <repo-url>
   cd Clean_Arc_template
   dotnet restore
   ```

2. **Configure database:**
   Update `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=cleanarcdb;Username=postgres;Password=your_password"
     }
   }
   ```

3. **Run migrations:**
   ```bash
   dotnet ef database update --project src/CleanArc.Infrastructure
   ```

4. **Start the API:**
   ```bash
   dotnet run --project src/Clean_Arc_template
   ```

5. **Verify it works:**
   - Open `https://localhost:5001/openapi/v1.json` (OpenAPI schema)
   - POST to `https://localhost:5001/api/orders` with:
     ```json
     {
       "customerId": "123e4567-e89b-12d3-a456-426614174000",
       "items": [
         {
           "productId": "123e4567-e89b-12d3-a456-426614174001",
           "quantity": 2,
           "price": 29.99
         }
       ]
     }
     ```

## 📝 Adding a New Feature

### Example: Add a "Cancel Order" Endpoint

1. **Add a domain method** (`CleanArc.Domain/Entities/Order.cs`):
   ```csharp
   public Result<Unit> Cancel()
   {
       if (Status != OrderStatus.Pending)
           return Result<Unit>.Failure("Only pending orders can be canceled");
       
       Status = OrderStatus.Canceled;
       return Result<Unit>.Success(Unit.Value);
   }
   ```

2. **Create a command** (`CleanArc.Application/Orders/Commands/CancelOrder/`):
   ```csharp
   public record CancelOrderCommand(Guid OrderId);

   public interface ICancelOrderHandler
   {
       Task<Result<Unit>> Handle(CancelOrderCommand command, CancellationToken ct);
   }

   public class CancelOrderHandler : ICancelOrderHandler
   {
       public async Task<Result<Unit>> Handle(CancelOrderCommand command, CancellationToken ct)
       {
           var order = await _context.Orders.FindAsync(new object[] { command.OrderId }, cancellationToken: ct);
           if (order is null)
               return Result<Unit>.Failure("Order not found");

           var result = order.Cancel();
           if (result.IsSuccess)
               await _context.SaveChangesAsync(ct);
           return result;
       }
   }
   ```

3. **Register the handler** (`Program.cs`):
   ```csharp
   builder.Services.AddScoped<ICancelOrderHandler, CancelOrderHandler>();
   ```

4. **Add the endpoint** (`Endpoints/OrderEndpoints.cs`):
   ```csharp
   group.MapDelete("/{id:guid}", CancelOrder)
       .WithName("CancelOrder");

   private static async Task<Ok> CancelOrder(
       Guid id,
       ICancelOrderHandler handler,
       CancellationToken ct)
   {
       var result = await handler.Handle(new CancelOrderCommand(id), ct);
       return result.IsSuccess
           ? TypedResults.Ok()
           : throw new InvalidOperationException(result.Error);
   }
   ```

5. **That's it!** `MapApiEndpoints()` auto-discovers your new endpoint.

## 🧪 Testing

### Run All Tests

```bash
dotnet test
```

### Architecture Tests

Verify layers don't depend on each other:
```bash
dotnet test --filter "ArchitectureTests"
```

### Unit/Integration Tests

Test individual handlers and use cases:
```bash
dotnet test --filter "OrderHandler"
```

## 🔐 Best Practices

### ✅ DO

- **Keep Domain pure:** No DbContext, no HTTP, no external services
- **Use interfaces for persistence:** `IAppDbContext` in Application, `AppDbContext` in Infrastructure
- **Return `Result<T>`:** For business logic outcomes, not exceptions for control flow
- **Validate early:** FluentValidation rules before handler execution
- **Write architecture tests:** Prevent creeping dependencies
- **One responsibility per handler:** A handler handles ONE use case
- **Use `CancellationToken`:** For async operations

### ❌ DON'T

- Import Infrastructure in Application layer
- Call DbContext directly in handlers (use interface)
- Use exceptions for expected business outcomes
- Hardcode configuration values
- Add business logic to endpoints
- Skip architecture tests
- Mix command and query logic in one handler

## 📊 Architecture Testing

The project includes NetArchTest.Rules to enforce:

```
✓ Domain doesn't depend on Application
✓ Domain doesn't depend on Infrastructure  
✓ Application doesn't depend on Infrastructure
✓ Infrastructure doesn't depend on API
✓ Layers have expected content (entities, handlers, DbContext)
```

Add tests before code and watch them pass as you build features.

## 🔄 Evolution Path

This architecture scales:

1. **Current (Small-Medium):** Single domain, one team
2. **Future (Large/Multi-team):** Split into [Modular Monolith](/docs/MODULAR_MONOLITH.md)
3. **Scale (Independent services):** Extract modules to microservices

## 📚 Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Result Pattern](https://github.com/ardalis/Result)
- [FluentValidation Docs](https://docs.fluentvalidation.net/)
- [Entity Framework Core Docs](https://docs.microsoft.com/en-us/ef/core/)

## 📄 License

This template is provided as-is for educational and commercial use.

---

**Questions?** See the architecture tests for implementation examples. The test suite is your documentation.
