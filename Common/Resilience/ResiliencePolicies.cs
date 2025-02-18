using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System;
using System.Net.Http;

namespace Common.Resilience
{
    public static class ResiliencePolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        // Log retry attempt
                        Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to {exception.Exception.Message}");
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30),
                    onBreak: (exception, duration) =>
                    {
                        // Log circuit breaker opening
                        Console.WriteLine($"Circuit breaker opened for {duration.TotalSeconds} seconds due to {exception.Exception.Message}");
                    },
                    onReset: () =>
                    {
                        // Log circuit breaker reset
                        Console.WriteLine("Circuit breaker reset");
                    });
        }

        public static AsyncRetryPolicy GetDatabaseRetryPolicy()
        {
            return Policy
                .Handle<DbUpdateException>()
                .Or<DbUpdateConcurrencyException>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        // Log retry attempt
                        Console.WriteLine($"Database retry {retryCount} after {timeSpan.TotalSeconds} seconds due to {exception.Message}");
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCombinedHttpPolicy()
        {
            return Policy.WrapAsync(GetHttpRetryPolicy(), GetHttpCircuitBreakerPolicy());
        }
    }
}