using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TestBankly.Domain.Interfaces;
using TestBankly.Infraestructure.Configurations;
using TestBankly.Infraestructure.Services;

namespace TestBankly.Api.Configurations
{
    public static class ServicesConfig
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = AppDomain.CurrentDomain.Load("TestBankly.Application");
            services.AddMediatR((conf) =>
            {
                conf.RegisterServicesFromAssembly(assembly);
            });

            services.AddScoped<ITransferAccountService, TransferAccountService>();
            services.AddHttpClient<TransferAccountService>();

            var serviceSettings = configuration.GetSection(typeof(ServiceSettings).Name).Get<ServiceSettings>();
            services.AddSingleton(serviceSettings);
            services.Configure<ServiceSettings>(configuration);
        }
    }
}
