using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        public int OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }

        public Property()
        {
            Title = string.Empty;
            Description = string.Empty;
            Status = "Available";
            CreatedAt = DateTime.UtcNow;
            Owner = new ApplicationUser();
            Bookings = new List<Booking>();
            Reviews = new List<Review>();
        }
    }
}