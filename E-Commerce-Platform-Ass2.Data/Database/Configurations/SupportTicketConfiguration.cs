using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(EntityTypeBuilder<SupportTicket> builder)
        {
            builder.ToTable("support_tickets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TicketCode)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.HasIndex(x => x.TicketCode)
                   .IsUnique();

            builder.Property(x => x.Subject)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.Description)
                   .IsRequired();

            builder.Property(x => x.Category)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Priority)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .IsRequired();

            builder.Property(x => x.ResponseCount)
                   .HasDefaultValue(0);

            builder.HasOne(x => x.Customer)
                   .WithMany()
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AssignedTo)
                   .WithMany()
                   .HasForeignKey(x => x.AssignedToId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.RelatedShop)
                   .WithMany()
                   .HasForeignKey(x => x.RelatedShopId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.RelatedOrder)
                   .WithMany()
                   .HasForeignKey(x => x.RelatedOrderId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.EscalatedByUser)
                   .WithMany()
                   .HasForeignKey(x => x.EscalatedByUserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(x => x.SlaStatus)
                   .HasMaxLength(20)
                   .HasDefaultValue("OnTrack");

            builder.Property(x => x.SlaLevel)
                   .HasMaxLength(20)
                   .HasDefaultValue("Standard");

            builder.Property(x => x.EscalationLevel)
                   .HasDefaultValue(0);

            builder.Property(x => x.FirstResponseMet)
                   .HasDefaultValue(false);

            builder.Property(x => x.ResolutionMet)
                   .HasDefaultValue(false);

            builder.Property(x => x.EscalationReason)
                   .HasMaxLength(500);

            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.Priority);
            builder.HasIndex(x => x.Category);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => x.SlaStatus);
            builder.HasIndex(x => x.EscalationLevel);
        }
    }
}
