using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NotificationService.Models;
using NotificationService.Interfaces;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<ActionResult> SendNotification([FromBody] NotificationDto notificationDto)
        {
            var result = await _notificationService.SendNotification(notificationDto);
            if (result) return Ok("Notification sent successfully.");
            return BadRequest("Failed to send notification.");
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetUserNotifications(int userId, [FromQuery] bool includeRead = false)
        {
            var notifications = await _notificationService.GetUserNotifications(userId, includeRead);
            return Ok(notifications);
        }

        [HttpGet("preferences/{userId}")]
        public async Task<ActionResult> GetUserPreferences(int userId)
        {
            var preferences = await _notificationService.GetUserPreferences(userId);
            if (preferences == null) return NotFound();
            return Ok(preferences);
        }

        [HttpPut("preferences/{userId}")]
        public async Task<ActionResult> UpdateNotificationPreferences(int userId, [FromBody] NotificationPreferences preferences)
        {
            var result = await _notificationService.UpdateNotificationPreferences(userId, preferences);
            if (result) return Ok();
            return BadRequest("Failed to update preferences.");
        }

        [HttpDelete("{notificationId}")]
        public async Task<ActionResult> DeleteNotification(int notificationId)
        {
            var result = await _notificationService.DeleteNotification(notificationId);
            if (result) return Ok();
            return NotFound();
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> SendBulkNotifications([FromBody] List<NotificationDto> notifications)
        {
            var result = await _notificationService.SendBulkNotifications(notifications);
            if (result) return Ok("Bulk notifications sent successfully.");
            return BadRequest("Failed to send bulk notifications.");
        }
    }
}