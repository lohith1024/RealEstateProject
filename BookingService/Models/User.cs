using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class User
    {
        public User()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}