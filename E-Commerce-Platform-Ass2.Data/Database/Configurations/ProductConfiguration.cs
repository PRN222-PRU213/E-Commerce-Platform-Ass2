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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Table
            builder.ToTable("products");

            // Primary key
            builder.HasKey(p => p.Id);

            // Columns
            builder.Property(p => p.ShopId)
                   .IsRequired();

            builder.Property(p => p.CategoryId)
                   .IsRequired();

            builder.Property(p => p.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(p => p.BasePrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.AvgRating)
                   .HasColumnType("decimal(3,2)")
                   .HasDefaultValue(0);

            builder.Property(p => p.ImageUrl)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Indexes
            builder.HasIndex(p => p.ShopId);
            builder.HasIndex(p => p.CategoryId);

            builder.HasIndex(p => p.Name);

            // Relationships
            builder.HasOne(p => p.Shop)
                   .WithMany(s => s.Products)
                   .HasForeignKey(p => p.ShopId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.ProductVariants)
                   .WithOne(pv => pv.Product)
                   .HasForeignKey(pv => pv.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reviews)
                   .WithOne(r => r.Product)
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
