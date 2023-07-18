using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace TestBankly.Api.Configurations
{
    public static class LoggingConfig
    {
        public static IServiceCollection AddLoggerSerilog(this IServiceCollection services)
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
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", true, true)
                    .Build();

                Log.Logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(configuration)
                  .CreateLogger();

                return Log.Logger;
            }
        }
    }
}
