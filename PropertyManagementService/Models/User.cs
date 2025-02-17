using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<Property> Properties { get; set; }

        public User()
        {
            Name = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            CreatedDate = DateTime.UtcNow;
            LastModifiedDate = DateTime.UtcNow;
            IsActive = true;
            Properties = new List<Property>();
        }
    }
}