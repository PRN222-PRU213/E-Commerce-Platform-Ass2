using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.TransactionType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(t => t.BalanceAfter)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(t => t.Description)
                   .HasMaxLength(500);

            builder.Property(t => t.ReferenceId)
                   .HasMaxLength(100);

            builder.Property(t => t.CreatedAt)
                   .IsRequired();

            builder.HasOne(t => t.Wallet)
                   .WithMany()
                   .HasForeignKey(t => t.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
