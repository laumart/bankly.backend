using TestBankly.Consumer.Worker;

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddHostedService<Worker>();
//    })
//    .Build();

//await host.RunAsync();

using MassTransit;
using Microsoft.AspNetCore.Builder;
using Serilog;
using TestBankly.Infraestructure.Configurations;
using TestBankly.Consumer.Worker.Workers;

try
{
    var builder = WebApplication.CreateBuilder(args);
    //builder.Services.AddLogger();
    builder.AddSerilog("Worker MassTransit");
    Log.Information("Starting Worker");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog(Log.Logger)
        .ConfigureServices((context, collection) =>
        {
            //var appSettings = new AppSettings();
            //context.Configuration.Bind(appSettings);
            //collection.AddOpenTelemetry(appSettings);

            //var assembly = AppDomain.CurrentDomain.Load("TestBankly.Application");
            //collection.AddMediatR((conf) =>
            //{
            //    conf.RegisterServicesFromAssembly(assembly);
            //});

            collection.AddHttpContextAccessor();

            collection.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();
                x.AddConsumer<QueueTransferConsumer>(typeof(QueueTransferConsumerDefinition));
                //x.AddRequestClient<ConvertEvent>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(context.Configuration.GetConnectionString("RabbitMq"));
                    cfg.UseDelayedMessageScheduler();
                    //cfg.ConnectReceiveObserver(new ReceiveObserverExtensions());
                    cfg.ServiceInstance(instance =>
                    {
                        instance.ConfigureJobServiceEndpoints();
                        instance.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("dev", false));
                    });
                });
            });
        }).Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}


public interface ConvertEvent
{
   
}