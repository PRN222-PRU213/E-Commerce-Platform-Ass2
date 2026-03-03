using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("chat_messages");
            
            builder.HasKey(m => m.Id);
            
            builder.Property(m => m.Id).IsRequired();
            builder.Property(m => m.ChatSessionId).IsRequired();
            
            builder.Property(m => m.SenderId).IsRequired(false);
            
            builder.Property(m => m.SenderRole)
                   .HasMaxLength(50)
                   .IsRequired();
                   
            builder.Property(m => m.Content)
                   .IsRequired();
                   
            builder.Property(m => m.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();
                   
            builder.Property(m => m.IsRead)
                   .HasDefaultValue(false)
                   .IsRequired();
            
            builder.HasOne(m => m.ChatSession)
                   .WithMany(s => s.Messages)
                   .HasForeignKey(m => m.ChatSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
