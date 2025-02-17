using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReviewRatingService.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public Review()
        {
            Comment = string.Empty;
            CreatedDate = DateTime.UtcNow;
            LastModifiedDate = DateTime.UtcNow;
            Property = new Property();
            User = new User();
        }
    }
}