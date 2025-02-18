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
        public bool PushNotifications { get; set; } = false;
        public bool SMSNotifications { get; set; } = false;

        public List<NotificationType> EnabledNotificationTypes { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public bool DoNotDisturb { get; set; }
        public string? TimeZone { get; set; } = "UTC";

        public string? PreferredLanguage { get; set; } = "en";

        public NotificationPreferences()
        {
            EmailAddress = string.Empty;
            PhoneNumber = string.Empty;
            EnabledNotificationTypes = new List<NotificationType>();
        }
    }
}