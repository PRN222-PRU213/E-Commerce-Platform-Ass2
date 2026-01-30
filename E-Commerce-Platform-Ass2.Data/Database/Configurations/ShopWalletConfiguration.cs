using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class ShopWalletConfiguration : IEntityTypeConfiguration<ShopWallet>
    {
        public void Configure(EntityTypeBuilder<ShopWallet> builder)
        {
            builder.ToTable("shop_wallets");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Balance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(w => w.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(w => w.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // One-to-One with Shop
            builder.HasOne(w => w.Shop)
                .WithOne()
                .HasForeignKey<ShopWallet>(w => w.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(w => w.ShopId).IsUnique();
        }
    }
}
