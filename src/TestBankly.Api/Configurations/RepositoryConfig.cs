using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TestBankly.Api.Consumer;
using TestBankly.Domain.Interfaces;
using TestBankly.Infraestructure.Repository;

namespace TestBankly.Api.Configurations
{
    public static class RepositoryConfig
    {
        public static void AddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Api.Data.TransactionContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<Api.Data.TransactionContext>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddMassTransit(bus =>
            {
                bus.AddDelayedMessageScheduler();
                bus.SetKebabCaseEndpointNameFormatter();

                bus.AddConsumer<QueueTransferConsumer>(typeof(QueueTransferConsumerDefinition));

                bus.UsingRabbitMq((ctx, busConfigurator) =>
                {
                    busConfigurator.Host(configuration.GetConnectionString("RabbitMq"));
                    busConfigurator.UseDelayedMessageScheduler();
                    busConfigurator.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("dev", false));
                    busConfigurator.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
                });
            });
        }
    }
}
