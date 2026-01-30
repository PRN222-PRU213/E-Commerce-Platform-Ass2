using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class ShopWalletTransactionConfiguration : IEntityTypeConfiguration<ShopWalletTransaction>
    {
        public void Configure(EntityTypeBuilder<ShopWalletTransaction> builder)
        {
            builder.ToTable("shop_wallet_transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TransactionType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.BalanceAfter)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.ReferenceId)
                .HasMaxLength(100);

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Relations
            builder.HasOne(t => t.ShopWallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.ShopWalletId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Order)
                .WithMany()
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(t => t.ShopWalletId);
            builder.HasIndex(t => t.CreatedAt);
        }
    }
}
