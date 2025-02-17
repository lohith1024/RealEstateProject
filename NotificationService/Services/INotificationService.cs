using System.Collections.Generic;
using System.Threading.Tasks;
using NotificationService.Models;

namespace NotificationService.Interfaces
{
    public interface INotificationService
    {
        // Notification sending methods
        Task<bool> SendEmailNotification(string email, string subject, string message);
        Task<bool> SendSmsNotification(string phoneNumber, string message);
        Task<bool> SendNotification(NotificationDto notification);
        Task<bool> SendBulkNotifications(List<NotificationDto> notifications);

        // Notification management methods
        Task<bool> MarkNotificationAsRead(int notificationId);
        Task<bool> DeleteNotification(int notificationId);
        Task<List<Notification>> GetUserNotifications(int userId, bool includeRead = false);
        Task<NotificationHistory> GetNotificationHistory(int userId);
        Task<List<Notification>> GetAllNotifications(int userId);

        // Preference management methods
        Task<NotificationPreferences> GetUserPreferences(int userId);
        Task<bool> UpdateNotificationPreferences(int userId, NotificationPreferences preferences);
    }
}