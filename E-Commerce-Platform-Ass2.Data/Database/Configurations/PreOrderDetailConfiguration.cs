using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class PreOrderDetailConfiguration : IEntityTypeConfiguration<PreOrderDetail>
    {
        public void Configure(EntityTypeBuilder<PreOrderDetail> builder)
        {
            builder.ToTable("preorder_details");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(x => x.ShopId).IsRequired();
            builder.Property(x => x.ExpectedAvailableDate).IsRequired();
            builder.Property(x => x.DepositPercent).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(x => x.DepositAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.RemainingAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.FinalPaymentDeadlineHours).HasDefaultValue(48).IsRequired();
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired();

            builder.HasIndex(x => x.OrderId).IsUnique();
            builder.HasIndex(x => new { x.ShopId, x.CreatedAt });
            builder.HasIndex(x => x.ExpectedAvailableDate);

            builder
                .HasOne(x => x.Order)
                .WithOne(o => o.PreOrderDetail)
                .HasForeignKey<PreOrderDetail>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Shop)
                .WithMany(s => s.PreOrderDetails)
                .HasForeignKey(x => x.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_preorder_details_DepositPercent",
                    "[DepositPercent] >= 1 AND [DepositPercent] <= 100"
                );
                t.HasCheckConstraint("CK_preorder_details_DepositAmount", "[DepositAmount] >= 0");
                t.HasCheckConstraint(
                    "CK_preorder_details_RemainingAmount",
                    "[RemainingAmount] >= 0"
                );
                t.HasCheckConstraint(
                    "CK_preorder_details_FinalPaymentDeadlineHours",
                    "[FinalPaymentDeadlineHours] > 0"
                );
            });
        }
    }
}
