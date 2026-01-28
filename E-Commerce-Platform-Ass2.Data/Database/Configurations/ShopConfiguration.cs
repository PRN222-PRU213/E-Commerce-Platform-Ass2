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
    public class ShopConfiguration : IEntityTypeConfiguration<Shop>
    {
        public void Configure(EntityTypeBuilder<Shop> builder)
        {
            builder.ToTable("shops");

            // Primary key
            builder.HasKey(s => s.Id);

            // Columns
            builder.Property(s => s.Id)
                   .IsRequired();

            builder.Property(s => s.UserId)
                   .IsRequired();

            builder.Property(s => s.ShopName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(s => s.Description)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(s => s.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(s => s.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Indexes
            builder.HasIndex(s => s.ShopName)
                   .IsUnique();

            builder.HasIndex(s => s.UserId);

            // Relationships
            builder.HasOne(s => s.User)
                   .WithOne()
                   .HasForeignKey<Shop>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Products)
                   .WithOne(p => p.Shop)
                   .HasForeignKey(p => p.ShopId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
