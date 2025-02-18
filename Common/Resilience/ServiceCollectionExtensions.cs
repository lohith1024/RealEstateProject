using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.Resilience
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddResiliencePolicies(this IServiceCollection services)
        {
            // Register typed HTTP clients with resilience policies
            services.AddHttpClient("default")
                .AddPolicyHandler(ResiliencePolicies.GetCombinedHttpPolicy());

            return services;
        }

        public static IHttpClientBuilder AddResiliencePolicy(this IHttpClientBuilder builder)
        {
            return builder.AddPolicyHandler(ResiliencePolicies.GetCombinedHttpPolicy());
        }

        public static IServiceCollection AddResilientHttpClient<TClient, TImplementation>(
            this IServiceCollection services,
            string baseAddress)
            where TClient : class
            where TImplementation : class, TClient
        {
            services.AddHttpClient<TClient, TImplementation>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
            })
            .AddResiliencePolicy();

            return services;
        }
    }
}