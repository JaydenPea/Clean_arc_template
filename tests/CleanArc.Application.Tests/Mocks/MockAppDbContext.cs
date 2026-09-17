namespace CleanArc.Application.Tests;

using CleanArc.Application.Common.Interfaces;
using CleanArc.Domain.Entities;
using CleanArc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class MockAppDbContext : IAppDbContext
{
    private readonly AppDbContext _context;

    public MockAppDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    public DbSet<Order> Orders => _context.Orders;

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
