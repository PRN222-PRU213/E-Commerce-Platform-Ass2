using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class PreOrderPolicyItemConfiguration : IEntityTypeConfiguration<PreOrderPolicyItem>
    {
        public void Configure(EntityTypeBuilder<PreOrderPolicyItem> builder)
        {
            builder.ToTable("preorder_policy_items");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductVariantId).IsRequired();
            builder.Property(x => x.AllowPreOrder).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.DepositPercent).HasColumnType("decimal(5,2)");
            builder.Property(x => x.Status).HasMaxLength(30).HasDefaultValue("Active").IsRequired();
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired();

            builder.HasIndex(x => x.ProductVariantId).IsUnique();
            builder.HasIndex(x => new { x.ProductVariantId, x.Status });

            builder
                .HasOne(x => x.ProductVariant)
                .WithOne(v => v.PreOrderPolicyItem)
                .HasForeignKey<PreOrderPolicyItem>(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_preorder_policy_items_DepositPercent",
                    "[DepositPercent] IS NULL OR ([DepositPercent] >= 1 AND [DepositPercent] <= 100)"
                );
                t.HasCheckConstraint(
                    "CK_preorder_policy_items_MaxPreOrderQty",
                    "[MaxPreOrderQty] IS NULL OR [MaxPreOrderQty] > 0"
                );
                t.HasCheckConstraint(
                    "CK_preorder_policy_items_LeadTimeDays",
                    "[LeadTimeDays] IS NULL OR [LeadTimeDays] > 0"
                );
            });
        }
    }
}
