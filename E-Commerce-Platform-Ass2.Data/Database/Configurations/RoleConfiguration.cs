using E_Commerce_Platform_Ass1.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass1.Data.Database.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Table name
            builder.ToTable("roles");

            // Primary key
            builder.HasKey(r => r.RoleId);

            // Columns
            builder.Property(r => r.RoleId)
                   .IsRequired();

            builder.Property(r => r.Name)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(r => r.Description)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Indexes
            builder.HasIndex(r => r.Name)
                   .IsUnique();
        }
    }
}
