using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway.Tests
{
    public class ApiGatewayTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public ApiGatewayTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task HealthCheck_Returns200()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SecurityHeaders_ArePresent()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            Assert.Contains("X-Content-Type-Options", response.Headers.Select(h => h.Key));
            Assert.Contains("X-Frame-Options", response.Headers.Select(h => h.Key));
            Assert.Contains("X-XSS-Protection", response.Headers.Select(h => h.Key));
        }

        [Theory]
        [InlineData("/api/booking/test")]
        [InlineData("/api/property/test")]
        [InlineData("/api/payment/test")]
        [InlineData("/api/users/test")]
        [InlineData("/api/notifications/test")]
        [InlineData("/api/search/test")]
        [InlineData("/api/reviews/test")]
        public async Task RouteEndpoints_ReturnExpectedStatusCode(string endpoint)
        {
            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            // Note: Since the actual services aren't running, we expect a 502 Bad Gateway
            Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        }

        [Fact]
        public async Task RateLimiting_EnforcesLimit()
        {
            // Arrange
            var endpoint = "/api/booking/test";
            var requests = new List<Task<HttpResponseMessage>>();

            // Act
            // Send 101 requests (exceeding our 100 per minute limit)
            for (int i = 0; i < 101; i++)
            {
                requests.Add(_client.GetAsync(endpoint));
            }

            var responses = await Task.WhenAll(requests);

            // Assert
            // At least one request should be rate limited (429 Too Many Requests)
            Assert.Contains(responses, r => r.StatusCode == HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task Cors_AllowsOptions()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/booking/test");
            request.Headers.Add("Origin", "http://example.com");
            request.Headers.Add("Access-Control-Request-Method", "GET");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Contains("Access-Control-Allow-Origin", response.Headers.Select(h => h.Key));
        }

        [Fact]
        public async Task ErrorHandling_ReturnsExpectedFormat()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Configure services to throw an exception for testing
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/error");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("error", content.ToLower());
        }
    }
}