using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Type)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(n => n.Link)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(n => n.IsRead)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Relationships
            builder.HasOne(n => n.User)
                   .WithMany()
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => n.UserId);
        }
    }
}
