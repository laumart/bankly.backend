using MassTransit;
using MassTransit.Metadata;
using MassTransit.RabbitMqTransport;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TestBankly.Application.Queries.Transfer;
using TestBankly.Domain;

namespace TestBankly.Api.Consumer
{
    public class QueueTransferConsumer : IConsumer<QueueTransferEvent>
    {
        private readonly ILogger<QueueTransferConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public QueueTransferConsumer(ILogger<QueueTransferConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory; 
        }

        public async Task Consume(ConsumeContext<QueueTransferEvent> context)
        {
            var timer = Stopwatch.StartNew();

            try
            {
                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<QueueTransferEvent>.ShortName);

                using var scope = _serviceScopeFactory.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var request = new TransferRequest
                {
                    AccountDestination = context.Message.AccountDestination,
                    AccountOrigin = context.Message.AccountOrigin,
                    Value = context.Message.Value
                };
                request.SetIdempotencyKey(context.Message.TransactionId);
                var response = await mediator.Send(request);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<QueueTransferEvent>.ShortName, ex);
            }
        }
    }

    public class QueueTransferConsumerDefinition : ConsumerDefinition<QueueTransferConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<QueueTransferConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
        }
    }
}
