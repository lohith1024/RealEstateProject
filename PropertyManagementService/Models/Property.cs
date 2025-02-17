using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementService.Models
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
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; }

        public int Bedrooms { get; set; }

        public int Bathrooms { get; set; }

        public decimal SquareFootage { get; set; }

        public int YearBuilt { get; set; }

        [Required]
        public PropertyType Type { get; set; }

        [Required]
        public PropertyStatus Status { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public DateTime ListedDate { get; set; }

        public DateTime? LastUpdated { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public virtual User Owner { get; set; } = null!;
        public virtual ICollection<PropertyImage> Images { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public Property()
        {
            Title = string.Empty;
            Description = string.Empty;
            Address = string.Empty;
            City = string.Empty;
            State = string.Empty;
            ZipCode = string.Empty;
            ListedDate = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
            Images = new List<PropertyImage>();
            Reviews = new List<Review>();
        }
    }
}