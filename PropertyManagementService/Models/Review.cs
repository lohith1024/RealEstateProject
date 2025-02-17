using System;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementService.Models
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
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        public virtual Property Property { get; set; } = null!;

        [Required]
        public virtual User User { get; set; } = null!;

        public Review()
        {
            Comment = string.Empty;
            CreatedDate = DateTime.UtcNow;
        }
    }
}