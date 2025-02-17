using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models
{
    public class NotificationPreferences
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool SMSNotifications { get; set; } = true;

        public List<NotificationType> EnabledNotificationTypes { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public bool DoNotDisturb { get; set; }
        public string TimeZone { get; set; }

        public NotificationPreferences()
        {
            EmailAddress = string.Empty;
            PhoneNumber = string.Empty;
            TimeZone = "UTC";
            EnabledNotificationTypes = new List<NotificationType>();
        }
    }
}