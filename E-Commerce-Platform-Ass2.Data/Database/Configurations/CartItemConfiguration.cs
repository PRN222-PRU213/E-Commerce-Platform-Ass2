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
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            // Primary key
            builder.HasKey(ci => ci.Id);

            // Properties
            builder.Property(ci => ci.Quantity)
                   .IsRequired();

            builder.Property(ci => ci.CreatedAt)
                   .IsRequired();

            // CartItem -> Cart (one-way)
            builder.HasOne(ci => ci.Cart)
                   .WithMany() // ❌ Cart KHÔNG có ICollection<CartItem>
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            // CartItem -> ProductVariant (one-way)
            builder.HasOne(ci => ci.ProductVariant)
                   .WithMany() // ❌ ProductVariant KHÔNG có ICollection<CartItem>
                   .HasForeignKey(ci => ci.ProductVariantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
