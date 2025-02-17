using System;
using System.ComponentModel.DataAnnotations;

namespace ReviewRatingService.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public Property()
        {
            Title = string.Empty;
            Description = string.Empty;
            Address = string.Empty;
            CreatedDate = DateTime.UtcNow;
            LastModifiedDate = DateTime.UtcNow;
        }
    }
}
