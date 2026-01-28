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
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            // Primary key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Status)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.CreatedAt)
                   .IsRequired();

            // Cart -> User (1 User có thể có nhiều Cart hoặc 1 Cart)
            builder.HasOne(c => c.User)
                   .WithMany() // ❌ nếu User KHÔNG có ICollection<Cart>
                               // .WithMany(u => u.Carts) // ✅ nếu User có ICollection<Cart>
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Cart -> CartItem (1 - N)
            builder.HasMany(c => c.CartItems)
                   .WithOne(ci => ci.Cart)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
