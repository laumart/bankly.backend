using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using System.IO;

namespace TestBankly.Infraestructure.Configurations
{
    public static class LogConfiguration
    {
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, string applicationName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ApplicationName", $"{applicationName}")
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                .WriteTo.Async(writeTo => writeTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"))
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Host.UseSerilog(Log.Logger, true);

            return builder;
        }

        public static IServiceCollection AddLogger(this IServiceCollection services)
        {
            var logger = LoggerManager.CreateLogger();
            services.AddSingleton(logger);
            return services;
        }

        public static class LoggerManager
        {
            public static Serilog.ILogger CreateLogger()
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                Log.Logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(configuration)
                  .CreateLogger();

                return Log.Logger;
            }
        }
    }
}
