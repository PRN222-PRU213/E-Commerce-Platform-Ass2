using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    [Table("support_ticket_replies")]
    public class SupportTicketReply
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }

        public Guid SenderId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SenderRole { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsInternal { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("TicketId")]
        public virtual SupportTicket Ticket { get; set; } = null!;

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; } = null!;
    }
}
