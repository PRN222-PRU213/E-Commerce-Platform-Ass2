using E_Commerce_Platform_Ass2.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Platform_Ass2.Data.Database.Configurations
{
    public class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
    {
        public void Configure(EntityTypeBuilder<ChatSession> builder)
        {
            builder.ToTable("chat_sessions");
            
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Id).IsRequired();
            builder.Property(c => c.CustomerId).IsRequired();
            builder.Property(c => c.ShopId).IsRequired();
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired();
            builder.Property(c => c.UpdatedAt).HasDefaultValueSql("GETDATE()").IsRequired();
            
            builder.HasOne(c => c.Customer)
                   .WithMany()
                   .HasForeignKey(c => c.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasOne(c => c.Shop)
                   .WithMany()
                   .HasForeignKey(c => c.ShopId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
