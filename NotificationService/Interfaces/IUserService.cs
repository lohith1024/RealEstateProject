using System.Threading.Tasks;
using NotificationService.Models;

namespace NotificationService.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserAsync(int userId);
        Task<NotificationPreferences?> GetUserPreferencesAsync(int userId);
    }
}