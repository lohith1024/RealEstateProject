using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using BookingService.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;

namespace BookingService.Services
{
    public class NotificationServiceClient : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NotificationServiceClient> _logger;
        private readonly ConcurrentQueue<(string Type, int BookingId)> _retryQueue;
        private readonly IAsyncPolicy<HttpResponseMessage> _fallbackPolicy;

        public NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _retryQueue = new ConcurrentQueue<(string Type, int BookingId)>();

            // Define fallback policy for all HTTP operations
            _fallbackPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .FallbackAsync(
                    fallbackValue: new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable),
                    onFallbackAsync: async (exception, _) =>
                    {
                        _logger.LogError(exception.Exception,
                            "Notification service failed. Using fallback.");
                        await Task.CompletedTask;
                    });
        }

        public async Task SendBookingConfirmationAsync(int bookingId)
        {
            var response = await _fallbackPolicy.ExecuteAsync(async () =>
                await _httpClient.PostAsync($"api/notifications/booking-confirmation/{bookingId}", null));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to send booking confirmation notification for BookingId: {BookingId}",
                    bookingId);
                _retryQueue.Enqueue(("BookingConfirmation", bookingId));
            }
            else
            {
                _logger.LogInformation(
                    "Booking confirmation notification sent successfully for BookingId: {BookingId}",
                    bookingId);
            }
        }

        public async Task SendBookingCancellationAsync(int bookingId)
        {
            var response = await _fallbackPolicy.ExecuteAsync(async () =>
                await _httpClient.PostAsync($"api/notifications/booking-cancellation/{bookingId}", null));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to send booking cancellation notification for BookingId: {BookingId}",
                    bookingId);
                _retryQueue.Enqueue(("BookingCancellation", bookingId));
            }
            else
            {
                _logger.LogInformation(
                    "Booking cancellation notification sent successfully for BookingId: {BookingId}",
                    bookingId);
            }
        }

        public async Task SendPaymentConfirmationAsync(int bookingId)
        {
            var response = await _fallbackPolicy.ExecuteAsync(async () =>
                await _httpClient.PostAsync($"api/notifications/payment-confirmation/{bookingId}", null));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to send payment confirmation notification for BookingId: {BookingId}",
                    bookingId);
                _retryQueue.Enqueue(("PaymentConfirmation", bookingId));
            }
            else
            {
                _logger.LogInformation(
                    "Payment confirmation notification sent successfully for BookingId: {BookingId}",
                    bookingId);
            }
        }

        public async Task ProcessQueuedNotificationsAsync()
        {
            while (_retryQueue.TryDequeue(out var notification))
            {
                bool success = false;
                try
                {
                    switch (notification.Type)
                    {
                        case "BookingConfirmation":
                            await SendBookingConfirmationAsync(notification.BookingId);
                            success = true;
                            break;
                        case "BookingCancellation":
                            await SendBookingCancellationAsync(notification.BookingId);
                            success = true;
                            break;
                        case "PaymentConfirmation":
                            await SendPaymentConfirmationAsync(notification.BookingId);
                            success = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing queued notification - Type: {Type}, BookingId: {BookingId}",
                        notification.Type, notification.BookingId);
                }

                if (!success)
                {
                    _retryQueue.Enqueue(notification);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }
    }
}