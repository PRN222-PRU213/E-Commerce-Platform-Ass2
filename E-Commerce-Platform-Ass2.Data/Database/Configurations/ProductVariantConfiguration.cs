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
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            // Table
            builder.ToTable("product_variants");

            // Primary key
            builder.HasKey(pv => pv.Id);

            // Columns
            builder.Property(pv => pv.ProductId)
                   .IsRequired();

            builder.Property(pv => pv.VariantName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(pv => pv.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(pv => pv.Size)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(pv => pv.Color)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(pv => pv.Stock)
                   .IsRequired();

            builder.Property(pv => pv.Sku)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(pv => pv.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(pv => pv.ImageUrl)
                   .HasMaxLength(255)
                   .IsRequired(false);

            // Indexes
            builder.HasIndex(pv => pv.ProductId);

            builder.HasIndex(pv => pv.Sku)
                   .IsUnique();

            builder.HasIndex(pv => new { pv.ProductId, pv.Size, pv.Color })
                   .IsUnique();

            // Relationships
            builder.HasOne(pv => pv.Product)
                   .WithMany(p => p.ProductVariants)
                   .HasForeignKey(pv => pv.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(pv => pv.CartItems)
                   .WithOne(ci => ci.ProductVariant)
                   .HasForeignKey(ci => ci.ProductVariantId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(pv => pv.OrderItems)
                   .WithOne(oi => oi.ProductVariant)
                   .HasForeignKey(oi => oi.ProductVariantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
