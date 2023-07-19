using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using System.Reflection;

namespace TestBankly.Api.Configurations
{
    public static class LoggingConfig
    {
        //public static IServiceCollection AddLoggerSerilog(this IServiceCollection services)
        //{
        //    var logger = LoggerManager.CreateLogger();
        //    services.AddSingleton(logger);
        //    return services;
        //}

        //public static class LoggerManager
        //{
            public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", true, true)
                    .Build();

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
                    .Enrich.WithProperty("Environment", environment)
                    .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                    .ReadFrom.Configuration(configuration)
                  .ReadFrom.Configuration(configuration)
                  .CreateLogger();

                builder.Logging.ClearProviders();
                builder.Host.UseSerilog(Log.Logger, true);

                return builder;
            }

            private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
            {
                return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                };
            }
        //}
    }



}
