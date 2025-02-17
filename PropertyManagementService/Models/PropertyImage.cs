using System;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementService.Models
{
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; }

        [StringLength(200)]
        public string Caption { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime UploadDate { get; set; }

        [Required]
        public virtual Property Property { get; set; } = null!;

        public PropertyImage()
        {
            ImageUrl = string.Empty;
            Caption = string.Empty;
            UploadDate = DateTime.UtcNow;
        }
    }
}