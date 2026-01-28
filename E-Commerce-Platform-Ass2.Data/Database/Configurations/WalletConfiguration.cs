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
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            // Primary key
            builder.HasKey(w => w.WalletId);

            // Properties
            builder.Property(w => w.Balance)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0);

            builder.Property(w => w.LastChangeAmount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(w => w.LastChangeType)
                   .HasMaxLength(100); // Ví dụ: "Deposit", "Withdraw", "Payment"

            builder.Property(w => w.UpdatedAt)
                   .IsRequired();

            builder.HasIndex(w => w.UserId)
                    .IsUnique();

            // Wallet -> User (Mối quan hệ 1-1)
            builder.HasOne(w => w.User)
                   .WithOne() // Giả định trong class User có property: public Wallet Wallet { get; set; }
                   .HasForeignKey<Wallet>(w => w.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
