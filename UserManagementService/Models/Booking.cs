using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        [Required]
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Property Property { get; set; }

        public Booking()
        {
            Status = "Pending";
            CreatedDate = DateTime.UtcNow;
            User = new User();
            Property = new Property();
        }
    }
}