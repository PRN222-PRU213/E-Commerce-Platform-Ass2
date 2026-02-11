using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    [Table("notifications")]
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "info"; // success, info, warning, error

        [Required]
        [MaxLength(255)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Link { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
