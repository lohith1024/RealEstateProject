using System.ComponentModel.DataAnnotations;

namespace ReviewRatingService.Models
{
    public class ReviewDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(1000)]
        public required string Comment { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}