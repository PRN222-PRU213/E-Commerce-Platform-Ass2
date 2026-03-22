using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    [Table("support_tickets")]
    public class SupportTicket
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public Guid? AssignedToId { get; set; }

        public Guid? RelatedShopId { get; set; }

        public Guid? RelatedOrderId { get; set; }

        [Required]
        [MaxLength(30)]
        public string TicketCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = "Khác";

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = "Medium";

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Open";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public int ResponseCount { get; set; } = 0;

        // SLA Fields
        public DateTime? FirstResponseAt { get; set; }

        public DateTime? FirstResponseDeadline { get; set; }

        public DateTime? ResolutionDeadline { get; set; }

        [MaxLength(20)]
        public string SlaStatus { get; set; } = "OnTrack"; // OnTrack, AtRisk, Breached

        [MaxLength(20)]
        public string SlaLevel { get; set; } = "Standard"; // Standard, Premium

        // Escalation Fields
        public int EscalationLevel { get; set; } = 0; // 0 = none, 1 = first escalation, 2 = second escalation

        public DateTime? LastEscalatedAt { get; set; }

        public Guid? EscalatedByUserId { get; set; }

        [MaxLength(500)]
        public string? EscalationReason { get; set; }

        public DateTime? ResolvedFirstAt { get; set; }

        public bool FirstResponseMet { get; set; } = false;

        public bool ResolutionMet { get; set; } = false;

        [ForeignKey("CustomerId")]
        public virtual User Customer { get; set; } = null!;

        [ForeignKey("AssignedToId")]
        public virtual User? AssignedTo { get; set; }

        [ForeignKey("RelatedShopId")]
        public virtual Shop? RelatedShop { get; set; }

        [ForeignKey("RelatedOrderId")]
        public virtual Order? RelatedOrder { get; set; }

        [ForeignKey("EscalatedByUserId")]
        public virtual User? EscalatedByUser { get; set; }

        public virtual ICollection<SupportTicketReply> Replies { get; set; } = new List<SupportTicketReply>();
    }
}
