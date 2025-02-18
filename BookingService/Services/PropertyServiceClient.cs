using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookingService.Interfaces;
using BookingService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;

namespace BookingService.Services
{
    public class PropertyServiceClient : IPropertyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PropertyServiceClient> _logger;
        private readonly IMemoryCache _cache;
        private readonly IAsyncPolicy<Property?> _fallbackPolicy;

        public PropertyServiceClient(HttpClient httpClient, ILogger<PropertyServiceClient> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;

            // Define fallback policy
            _fallbackPolicy = Policy<Property?>
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .FallbackAsync(
                    fallbackValue: null,
                    onFallbackAsync: async (exception, _) =>
                    {
                        _logger.LogError(exception.Exception,
                            "Property service failed. Fallback policy activated.");
                        await Task.CompletedTask;
                    });
        }

        public async Task<Property?> GetPropertyAsync(int propertyId)
        {
            var cacheKey = $"property_{propertyId}";
            if (_cache.TryGetValue<Property>(cacheKey, out var cachedProperty))
            {
                return cachedProperty;
            }

            return await _fallbackPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var property = await _httpClient.GetFromJsonAsync<Property>($"api/properties/{propertyId}");
                    if (property != null)
                    {
                        var cacheOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                        _cache.Set(cacheKey, property, cacheOptions);
                        _logger.LogInformation(
                            "Property details retrieved successfully for PropertyId: {PropertyId}",
                            propertyId);
                        return property;
                    }

                    _logger.LogWarning(
                        "Property not found for PropertyId: {PropertyId}",
                        propertyId);
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error retrieving property details for PropertyId: {PropertyId}",
                        propertyId);
                    throw;
                }
            });
        }
    }
}