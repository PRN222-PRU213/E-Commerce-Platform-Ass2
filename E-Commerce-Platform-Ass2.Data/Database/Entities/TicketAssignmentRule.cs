using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    [Table("ticket_assignment_rules")]
    public class TicketAssignmentRule
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string RuleName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Category { get; set; } = "All";

        [MaxLength(20)]
        public string Priority { get; set; } = "All";

        [MaxLength(20)]
        public string TicketStatus { get; set; } = "Open";

        public bool IsActive { get; set; } = true;

        public int Priority_Order { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Guid CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; } = null!;

        public Guid? AssignedToId { get; set; }

        [ForeignKey("AssignedToId")]
        public virtual User? AssignedTo { get; set; }

        public Guid? AssignedToRoleId { get; set; }

        [ForeignKey("AssignedToRoleId")]
        public virtual Role? AssignedToRole { get; set; }
    }
}
