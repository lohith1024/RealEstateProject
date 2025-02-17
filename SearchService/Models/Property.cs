using System;
using System.Collections.Generic;

namespace SearchService.Models
{
    public class Property
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int OwnerId { get; set; }
        public PropertyType Type { get; set; }
        public PropertyStatus Status { get; set; }
        public required ICollection<Booking> Bookings { get; set; }
    }
}