using E_Commerce_Platform_Ass1.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass1.Data.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table name
            builder.ToTable("users");

            // Primary key
            builder.HasKey(u => u.Id);

            // Columns
            builder.Property(u => u.Id)
                   .IsRequired();

            builder.Property(u => u.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.PasswordHash)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(u => u.Email)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(u => u.RoleId)
                   .IsRequired();

            builder.Property(u => u.Status)
                   .HasDefaultValue(true)
                   .IsRequired();

            // Foreign key relationship
            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(u => u.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Indexes
            builder.HasIndex(u => u.Email)
                   .IsUnique();

            // Email Verification
            builder.Property(u => u.EmailVerified)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(u => u.EmailVerificationToken)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(u => u.EmailVerificationTokenExpiry)
                   .IsRequired(false);

            builder.HasIndex(u => u.EmailVerificationToken)
                   .IsUnique()
                   .HasFilter("[EmailVerificationToken] IS NOT NULL");        }
    }
}
