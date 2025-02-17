using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotificationService.Models;
using NotificationService.Interfaces;
using NotificationService.Data;
using MailKit.Net.Smtp;
using MimeKit;

namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public NotificationService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> SendEmailNotification(string email, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Real Estate App", _configuration["Email:From"]));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("plain") { Text = message };

                using var client = new SmtpClient();
                await client.ConnectAsync(_configuration["Email:SmtpServer"],
                    int.Parse(_configuration["Email:Port"]), false);
                await client.AuthenticateAsync(_configuration["Email:Username"],
                    _configuration["Email:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendSmsNotification(string phoneNumber, string message)
        {
            // Implement SMS sending logic here using Twilio
            return true;
        }

        public async Task<bool> SendNotification(NotificationDto notification)
        {
            try
            {
                var notificationRecord = new Notification
                {
                    UserId = notification.UserId,
                    Title = notification.Title,
                    Message = notification.Message,
                    Type = notification.Type,
                    RelatedEntityType = notification.RelatedEntityType,
                    RelatedEntityId = notification.RelatedEntityId,
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false
                };

                _context.Notifications.Add(notificationRecord);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ManageNotificationPreferences(int userId, NotificationPreferences preferences)
        {
            var existingPreferences = await _context.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingPreferences == null)
            {
                preferences.UserId = userId;
                _context.NotificationPreferences.Add(preferences);
            }
            else
            {
                existingPreferences.EmailNotifications = preferences.EmailNotifications;
                existingPreferences.PushNotifications = preferences.PushNotifications;
                existingPreferences.SMSNotifications = preferences.SMSNotifications;
                existingPreferences.EnabledNotificationTypes = preferences.EnabledNotificationTypes;
                existingPreferences.EmailAddress = preferences.EmailAddress;
                existingPreferences.PhoneNumber = preferences.PhoneNumber;
                existingPreferences.DoNotDisturb = preferences.DoNotDisturb;
                existingPreferences.TimeZone = preferences.TimeZone;
                _context.NotificationPreferences.Update(existingPreferences);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<NotificationHistory> GetNotificationHistory(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return new NotificationHistory
            {
                UserId = userId,
                Notifications = notifications,
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task<List<Notification>> GetAllNotifications(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> MarkNotificationAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            notification.ReadDate = DateTime.UtcNow;
            _context.Notifications.Update(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Notification>> GetUserNotifications(int userId, bool includeRead = false)
        {
            var query = _context.Notifications.Where(n => n.UserId == userId);
            if (!includeRead)
            {
                query = query.Where(n => !n.IsRead);
            }
            return await query.OrderByDescending(n => n.CreatedDate).ToListAsync();
        }

        public async Task<NotificationPreferences> GetUserPreferences(int userId)
        {
            return await _context.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<bool> UpdateNotificationPreferences(int userId, NotificationPreferences preferences)
        {
            return await ManageNotificationPreferences(userId, preferences);
        }

        public async Task<bool> DeleteNotification(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SendBulkNotifications(List<NotificationDto> notifications)
        {
            try
            {
                var notificationRecords = notifications.Select(n => new Notification
                {
                    UserId = n.UserId,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    RelatedEntityType = n.RelatedEntityType,
                    RelatedEntityId = n.RelatedEntityId,
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false
                }).ToList();

                await _context.Notifications.AddRangeAsync(notificationRecords);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}