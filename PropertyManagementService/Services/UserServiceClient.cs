using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PropertyManagementService.Interfaces;
using PropertyManagementService.Models;

namespace PropertyManagementService.Services
{
    public class UserServiceClient : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User?> GetUserAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<User>($"api/user/{userId}");
        }

        public async Task<bool> ValidateUserAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/user/{userId}/validate");
            return response.IsSuccessStatusCode;
        }
    }
}