using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
    {
        public void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {
            builder.ToTable("ReturnRequests");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.RequestType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Reason)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.ReasonDetail)
                .HasMaxLength(1000);

            builder.Property(r => r.EvidenceImages)
                .HasMaxLength(2000);

            builder.Property(r => r.RequestedAmount)
                .HasPrecision(18, 2);

            builder.Property(r => r.ApprovedAmount)
                .HasPrecision(18, 2);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.ShopResponse)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(r => r.Order)
                .WithMany()
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ProcessedByShop)
                .WithMany()
                .HasForeignKey(r => r.ProcessedByShopId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(r => r.OrderId);
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.ProcessedByShopId);
        }
    }
}
