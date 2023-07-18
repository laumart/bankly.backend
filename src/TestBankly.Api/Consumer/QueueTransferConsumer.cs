using MassTransit;
using MassTransit.Metadata;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TestBankly.Application.Queries.Transfer;

namespace TestBankly.Api.Consumer
{
    public class QueueTransferConsumer : IConsumer<TransferRequest>
    {
        private readonly ILogger<QueueTransferConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public QueueTransferConsumer(ILogger<QueueTransferConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory; 
        }

        public async Task Consume(ConsumeContext<TransferRequest> context)
        {
            var timer = Stopwatch.StartNew();

            try
            {
                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TransferRequest>.ShortName);

                using var scope = _serviceScopeFactory.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var response = await mediator.Send(context.Message);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TransferRequest>.ShortName, ex);
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
