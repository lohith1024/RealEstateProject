using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NotificationService.Interfaces;
using NotificationService.Models;

namespace NotificationService.Services
{
    public class UserServiceClient : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDto?> GetUserAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return null;
        }

        public async Task<NotificationPreferences?> GetUserPreferencesAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}/preferences");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<NotificationPreferences>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return null;
        }
    }
}
