using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Property
    {
        public Property()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [MaxLength(2000)]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxGuests { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int OwnerId { get; set; }

        public PropertyType Type { get; set; }

        public PropertyStatus Status { get; set; }

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}