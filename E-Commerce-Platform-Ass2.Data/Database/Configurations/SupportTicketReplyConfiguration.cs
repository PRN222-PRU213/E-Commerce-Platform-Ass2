using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class SupportTicketReplyConfiguration : IEntityTypeConfiguration<SupportTicketReply>
    {
        public void Configure(EntityTypeBuilder<SupportTicketReply> builder)
        {
            builder.ToTable("support_ticket_replies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SenderRole)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.Content)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .IsRequired();

            builder.HasOne(x => x.Ticket)
                   .WithMany(x => x.Replies)
                   .HasForeignKey(x => x.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Sender)
                   .WithMany()
                   .HasForeignKey(x => x.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TicketId);
            builder.HasIndex(x => x.SenderId);
        }
    }
}
