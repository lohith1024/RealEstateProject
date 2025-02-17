using System.ComponentModel.DataAnnotations;

namespace PropertyManagementService.Models
{
    public class PropertyDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public PropertyDto()
        {
            Title = string.Empty;
            Description = string.Empty;
            Location = string.Empty;
        }
    }
}