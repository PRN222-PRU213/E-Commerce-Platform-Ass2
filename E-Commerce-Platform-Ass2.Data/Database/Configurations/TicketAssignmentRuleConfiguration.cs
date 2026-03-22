using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class TicketAssignmentRuleConfiguration : IEntityTypeConfiguration<TicketAssignmentRule>
    {
        public void Configure(EntityTypeBuilder<TicketAssignmentRule> builder)
        {
            builder.ToTable("ticket_assignment_rules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RuleName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Category)
                   .HasMaxLength(50)
                   .HasDefaultValue("All");

            builder.Property(x => x.Priority)
                   .HasMaxLength(20)
                   .HasDefaultValue("All");

            builder.Property(x => x.TicketStatus)
                   .HasMaxLength(20)
                   .HasDefaultValue("Open");

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.Property(x => x.Priority_Order)
                   .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.CreatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedById)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.AssignedTo)
                   .WithMany()
                   .HasForeignKey(x => x.AssignedToId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.AssignedToRole)
                   .WithMany()
                   .HasForeignKey(x => x.AssignedToRoleId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.IsActive);
            builder.HasIndex(x => x.Category);
            builder.HasIndex(x => x.Priority);
            builder.HasIndex(x => x.Priority_Order);
        }
    }
}
