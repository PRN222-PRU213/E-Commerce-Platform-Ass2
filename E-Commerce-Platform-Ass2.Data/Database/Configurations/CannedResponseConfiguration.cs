using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class CannedResponseConfiguration : IEntityTypeConfiguration<CannedResponse>
    {
        public void Configure(EntityTypeBuilder<CannedResponse> builder)
        {
            builder.ToTable("canned_responses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Content)
                   .IsRequired();

            builder.Property(x => x.Category)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Priority)
                   .HasMaxLength(20)
                   .HasDefaultValue("All");

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.Property(x => x.SortOrder)
                   .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.CreatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Category);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
