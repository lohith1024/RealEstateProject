using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using BookingService.Interfaces;
using BookingService.Services;
using Common.Resilience;
using Common.Logging;
using Microsoft.EntityFrameworkCore;
using BookingService.Data;
using BookingService.Repositories;
using Serilog;

namespace BookingService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingService", Version = "v1" });
            });

            services.AddDbContext<BookingDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMemoryCache();
            services.AddLogging();

            // Add Serilog
            services.AddCustomLogging(Configuration, "BookingService");

            services.AddResiliencePolicies();

            services.AddResilientHttpClient<IPaymentService, PaymentServiceClient>(
                Configuration["ServiceUrls:PaymentService"]);

            services.AddResilientHttpClient<IPropertyService, PropertyServiceClient>(
                Configuration["ServiceUrls:PropertyService"]);

            services.AddResilientHttpClient<INotificationService, NotificationServiceClient>(
                Configuration["ServiceUrls:NotificationService"]);

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<Interfaces.IBookingService, Services.BookingService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookingService v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            // Use Serilog request logging
            app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}