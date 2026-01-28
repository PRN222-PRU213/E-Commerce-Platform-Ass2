using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass1.Data.Database.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Table
            builder.ToTable("orders");

            // Primary key
            builder.HasKey(o => o.Id);

            // Columns
            builder.Property(o => o.UserId)
                   .IsRequired();

            builder.Property(o => o.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(o => o.ShippingAddress)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(o => o.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(o => o.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Indexes
            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.CreatedAt);

            // Relationships

            // Order -> User (N - 1)
            builder.HasOne(o => o.User)
                   .WithMany()
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order -> OrderItems (1 - N)
            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Order -> Shipments (1 - N)
            builder.HasMany(o => o.Shipments)
                   .WithOne(s => s.Order)
                   .HasForeignKey(s => s.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Order -> Payments (1 - N)
            builder.HasMany(o => o.Payments)
                   .WithOne(p => p.Order)
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
