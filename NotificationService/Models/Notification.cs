using System;

namespace NotificationService.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public string RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }

        public Notification()
        {
            Title = string.Empty;
            Message = string.Empty;
            RelatedEntityType = string.Empty;
            CreatedDate = DateTime.UtcNow;
        }
    }
}