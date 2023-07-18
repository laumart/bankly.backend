using MassTransit;
using MassTransit.Metadata;
using MediatR;
using System.Diagnostics;
using TestBankly.Application.Queries.Transfer;

namespace TestBankly.Consumer.Worker.Workers
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
                using var scope = _serviceScopeFactory.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var response = await mediator.Send(context.Message);
                
                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TransferRequest>.ShortName);
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
