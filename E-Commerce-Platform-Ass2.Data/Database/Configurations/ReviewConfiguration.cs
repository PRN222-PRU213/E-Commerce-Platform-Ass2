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
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // Table
            builder.ToTable("reviews");

            // Primary key
            builder.HasKey(r => r.Id);

            // Columns
            builder.Property(r => r.UserId)
                   .IsRequired();

            builder.Property(r => r.ProductId)
                   .IsRequired();

            builder.Property(r => r.OrderItemId)
                   .IsRequired();

            builder.Property(r => r.Rating)
                   .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(r => r.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(r => r.SpamScore)
                   .HasColumnType("decimal(5,2)")
                   .HasDefaultValue(0);

            builder.Property(r => r.ToxicityScore)
                   .HasColumnType("decimal(5,2)")
                   .HasDefaultValue(0);

            builder.Property(r => r.ModerationReason)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(r => r.ModeratedAt)
                   .IsRequired(false);

            builder.Property(r => r.ModeratedBy)
                   .IsRequired(false);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Indexes
            builder.HasIndex(r => r.ProductId);
            builder.HasIndex(r => r.UserId);

            builder.HasIndex(r => r.OrderItemId)
                   .IsUnique(); // 1 OrderItem chỉ được review 1 lần

            // Relationships (one-way navigation)
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Product)
                   .WithMany()
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.OrderItem)
                   .WithOne()
                   .HasForeignKey<Review>(r => r.OrderItemId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
