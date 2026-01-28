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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Table
            builder.ToTable("order_items");

            // Primary key
            builder.HasKey(oi => oi.Id);

            // Columns
            builder.Property(oi => oi.OrderId)
                   .IsRequired();

            builder.Property(oi => oi.ProductVariantId)
                   .IsRequired();

            builder.Property(oi => oi.ProductName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(oi => oi.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(oi => oi.Quantity)
                   .IsRequired();

            // Indexes
            builder.HasIndex(oi => oi.OrderId);
            builder.HasIndex(oi => oi.ProductVariantId);

            // Relationships
            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.ProductVariant)
                   .WithMany(pv => pv.OrderItems)
                   .HasForeignKey(oi => oi.ProductVariantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(oi => oi.Reviews)
                   .WithOne(r => r.OrderItem)
                   .HasForeignKey(r => r.OrderItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
