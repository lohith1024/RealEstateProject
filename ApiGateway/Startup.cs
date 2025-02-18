using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Transforms;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add logging
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        // Add CORS
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        // Add Rate Limiting
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        // Add Health Checks
        services.AddHealthChecks();

        // Add Reverse Proxy with detailed logging
        services.AddReverseProxy()
            .LoadFromConfig(Configuration.GetSection("ReverseProxy"))
            .AddTransforms(transforms =>
            {
                transforms.AddRequestTransform(async transformContext =>
                {
                    var logger = transformContext.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation("Proxying request: {Method} {Path}",
                        transformContext.HttpContext.Request.Method,
                        transformContext.HttpContext.Request.Path);
                });
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            logger.LogInformation("Running in Development mode");
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        // Global error handling
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred" });
            });
        });

        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseRateLimiter();

        // Add basic security headers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            await next();
        });

        // Add default route handler
        app.Use(async (context, next) =>
        {
            logger.LogInformation("Request received for: {Path}", context.Request.Path);
            await next();
            if (context.Response.StatusCode == 404)
            {
                logger.LogWarning("No route found for: {Path}", context.Request.Path);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = $"No route found for {context.Request.Path}" });
            }
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapGet("/", async context =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    status = "running",
                    version = "1.0",
                    endpoints = new[] {
                        "/health",
                        "/api/booking/*",
                        "/api/property/*",
                        "/api/payment/*",
                        "/api/users/*",
                        "/api/notifications/*",
                        "/api/search/*",
                        "/api/reviews/*"
                    }
                });
            });
            endpoints.MapReverseProxy(proxyPipeline =>
            {
                proxyPipeline.Use(async (context, next) =>
                {
                    logger.LogInformation("Proxying request to: {Path}", context.Request.Path);
                    await next();
                });
            });
        });
    }
}