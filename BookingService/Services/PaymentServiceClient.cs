using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookingService.Interfaces;
using BookingService.Models;
using Microsoft.Extensions.Logging;
using Polly;

namespace BookingService.Services
{
    public class PaymentServiceClient : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentServiceClient> _logger;
        private readonly IAsyncPolicy<PaymentResult> _fallbackPolicy;

        public PaymentServiceClient(HttpClient httpClient, ILogger<PaymentServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Define fallback policy
            _fallbackPolicy = Policy<PaymentResult>
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .FallbackAsync(
                    fallbackValue: new PaymentResult
                    {
                        Success = false,
                        ErrorMessage = "Payment service is currently unavailable. Please try again later."
                    },
                    onFallbackAsync: async (exception, _) =>
                    {
                        _logger.LogError(exception.Exception,
                            "Payment service failed. Fallback policy activated.");
                        await Task.CompletedTask;
                    });
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            return await _fallbackPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("api/payments", request);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<PaymentResult>();
                        _logger.LogInformation(
                            "Payment processed successfully for BookingId: {BookingId}, Amount: {Amount}",
                            request.BookingId, request.Amount);
                        return result ?? new PaymentResult { Success = false };
                    }

                    _logger.LogWarning(
                        "Failed to process payment for BookingId: {BookingId}, Amount: {Amount}",
                        request.BookingId, request.Amount);
                    return new PaymentResult { Success = false };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error processing payment for BookingId: {BookingId}, Amount: {Amount}",
                        request.BookingId, request.Amount);
                    throw;
                }
            });
        }
    }
}