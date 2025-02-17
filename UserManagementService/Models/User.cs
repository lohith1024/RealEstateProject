using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public bool IsVerified { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public ICollection<Property> Properties { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }

        public User()
        {
            Username = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            PhoneNumber = string.Empty;
            ProfilePictureUrl = string.Empty;
            Role = UserRole.RegularUser;
            CreatedDate = DateTime.UtcNow;
            IsActive = true;
            Properties = new List<Property>();
            Bookings = new List<Booking>();
            Reviews = new List<Review>();
        }
    }
}