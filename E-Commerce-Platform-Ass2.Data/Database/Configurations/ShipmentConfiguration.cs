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
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            // Table
            builder.ToTable("shipments");

            // Primary key
            builder.HasKey(s => s.Id);

            // Columns
            builder.Property(s => s.OrderId)
                   .IsRequired();

            builder.Property(s => s.Carrier)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(s => s.TrackingCode)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(s => s.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(s => s.UpdatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Indexes
            builder.HasIndex(s => s.OrderId)
                   .IsUnique(); // 1 Order chỉ có 1 Shipment

            builder.HasIndex(s => s.TrackingCode)
                   .IsUnique();

            // Relationship: Shipment -> Order (1 - 1, one-way)
            builder.HasOne(s => s.Order)
                   .WithOne()
                   .HasForeignKey<Shipment>(s => s.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
