namespace CleanArc.Infrastructure.Persistence.Configurations;

using CleanArc.Domain.Entities;
using CleanArc.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(o => o.Status)
            .HasConversion<int>()
            .HasDefaultValue(OrderStatus.Pending);

        builder.Property(o => o.Total)
            .HasPrecision(18, 2);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.OwnsMany(o => o.Items, ownedBuilder =>
        {
            ownedBuilder.HasKey("Id");

            ownedBuilder.Property<Guid>("OrderId");
            ownedBuilder.WithOwner()
                .HasForeignKey("OrderId");

            ownedBuilder.Property(i => i.ProductId)
                .IsRequired()
                .HasMaxLength(256);

            ownedBuilder.Property(i => i.Quantity)
                .IsRequired();

            ownedBuilder.Property(i => i.UnitPrice)
                .HasPrecision(18, 2);
        });

        builder.ToTable("Orders");
    }
}
