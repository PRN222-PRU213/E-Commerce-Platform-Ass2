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
    public class RefundConfiguration : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> builder)
        {
            builder.HasKey(r => r.Id);

            // 2. Properties configuration
            builder.Property(r => r.RequestId)
                   .IsRequired()
                   .HasMaxLength(100); // Thường RequestId từ cổng thanh toán có độ dài cố định

            builder.Property(r => r.RefundAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)"); // Đảm bảo độ chính xác cho tiền tệ

            builder.Property(r => r.Reason)
                   .HasMaxLength(500); // Reason có thể null nhưng nên giới hạn độ dài

            builder.Property(r => r.Status)
                   .IsRequired()
                   .HasMaxLength(50); // Success, Failed, Pending...

            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            builder.HasOne(r => r.Payment)
                   .WithOne() 
                   .HasForeignKey<Refund>(r => r.PaymentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
