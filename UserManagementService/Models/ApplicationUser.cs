using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace UserManagementService.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public UserRole Role { get; set; }

        // Navigation properties
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public ApplicationUser()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            ProfilePictureUrl = string.Empty;
            Role = UserRole.RegularUser;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Properties = new List<Property>();
            Bookings = new List<Booking>();
            Reviews = new List<Review>();
        }
    }
}