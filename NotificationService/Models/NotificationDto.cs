using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models
{
    public class NotificationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }

        public NotificationDto()
        {
            Title = string.Empty;
            Message = string.Empty;
            RelatedEntityType = string.Empty;
        }
    }
}