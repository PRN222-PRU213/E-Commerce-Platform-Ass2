using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class ReviewViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty;
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        [Required]
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;

        public string RatingStars => new string('★', Rating);

        public string CreatedAtFormatted =>
            CreatedAt.ToString("dd/MM/yyyy HH:mm");

        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime? ModerateAt { get; set; }
    }
}
