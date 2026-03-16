using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Table
            builder.ToTable("payments");

            // Primary key
            builder.HasKey(p => p.Id);

            // Columns
            builder.Property(p => p.OrderId).IsRequired();

            builder.Property(p => p.Method).HasMaxLength(50).IsRequired();

            builder
                .Property(p => p.PaymentStage)
                .HasMaxLength(30)
                .HasDefaultValue("FULL")
                .IsRequired();

            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(p => p.Status).HasMaxLength(50).IsRequired();

            builder.Property(p => p.TransactionCode).HasMaxLength(100).IsRequired();

            builder.Property(p => p.PaidAt).IsRequired();

            // Indexes
            builder.HasIndex(p => p.OrderId);

            builder.HasIndex(p => new
            {
                p.OrderId,
                p.PaymentStage,
                p.PaidAt,
            });

            builder.HasIndex(p => p.TransactionCode).IsUnique();

            // Relationship: Payment -> Order (N - 1)
            builder
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_payments_PaymentStage",
                    "[PaymentStage] IN ('DEPOSIT','FINAL','FULL')"
                );
            });
        }
    }
}
