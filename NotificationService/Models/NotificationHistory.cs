using System;
using System.Collections.Generic;

namespace NotificationService.Models
{
    public class NotificationHistory
    {
        public int UserId { get; set; }
        public List<Notification> Notifications { get; set; }
        public DateTime LastUpdated { get; set; }

        public NotificationHistory()
        {
            Notifications = new List<Notification>();
            LastUpdated = DateTime.UtcNow;
        }
    }
}