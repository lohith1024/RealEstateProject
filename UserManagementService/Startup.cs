using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Consul;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Net.Http;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagementService.Data;
using UserManagementService.Models;
using UserManagementService.Services;
using UserManagementService.Interfaces;

namespace UserManagementService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddHttpClient("PropertyService", client =>
            {
                client.BaseAddress = new Uri(Configuration["ServiceUrls:PropertyService"]);
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30)));

            services.AddScoped<IUserService, UserService>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "User Management Service API",
                    Version = "v1",
                    Description = "API for managing users in the real estate application"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // Add Consul client
            services.AddSingleton<IConsulServiceClient>(sp =>
            {
                var consulAddress = Configuration.GetValue<string>("Consul:Address");
                return new ConsulServiceClient(new Uri(consulAddress));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management Service API v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Register service with Consul
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulServiceClient>();
            lifetime.ApplicationStarted.Register(async () =>
            {
                await consulClient.RegisterServiceAsync();
            });

            lifetime.ApplicationStopping.Register(async () =>
            {
                await consulClient.DeregisterServiceAsync();
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient>(sp => new ConsulClient(cfg =>
            {
                var consulUrl = configuration["Consul:Url"];
                cfg.Address = new Uri(consulUrl);
            }));

            return services;
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

            var serviceName = configuration["Service:Name"];
            var serviceId = configuration["Service:Id"];
            var serviceAddress = configuration["Service:Address"];
            var servicePort = int.Parse(configuration["Service:Port"]);

            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Address = serviceAddress,
                Port = servicePort,
                Check = new AgentServiceCheck
                {
                    HTTP = $"http://{serviceAddress}:{servicePort}/health",
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceId).Wait();
            });

            return app;
        }
    }
}