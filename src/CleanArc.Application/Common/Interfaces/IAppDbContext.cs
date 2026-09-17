namespace CleanArc.Application.Common.Interfaces;

using CleanArc.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface IAppDbContext
{
    DbSet<Order> Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
