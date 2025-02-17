using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Property Property { get; set; }

        public Review()
        {
            Comment = string.Empty;
            CreatedAt = DateTime.UtcNow;
            User = new User();
            Property = new Property();
        }
    }
}