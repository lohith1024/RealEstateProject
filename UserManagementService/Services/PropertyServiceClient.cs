using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UserManagementService.Interfaces;
using UserManagementService.Models;

namespace UserManagementService.Services
{
    public class PropertyServiceClient : IPropertyService
    {
        private readonly HttpClient _httpClient;

        public PropertyServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Property>> GetUserPropertiesAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<List<Property>>($"api/property/user/{userId}") ?? new List<Property>();
        }

        public async Task<Property?> GetPropertyDetailsAsync(int propertyId)
        {
            return await _httpClient.GetFromJsonAsync<Property>($"api/property/{propertyId}");
        }
    }
}