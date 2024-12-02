using BasketConfirmed;
using Confluent.Kafka;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading;

namespace DeliveryApp.Api.Adapters.Kafka.BasketConfirmed
{
    public class ConsumerService : BackgroundService
    {
        private readonly IMediator _mediator;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _topic;
        public ConsumerService(IMediator mediator, IOptions<Settings> settings)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            if (string.IsNullOrWhiteSpace(settings.Value.MESSAGE_BROKER_HOST)) throw new ArgumentException(nameof(settings.Value.MESSAGE_BROKER_HOST));
            if (string.IsNullOrWhiteSpace(settings.Value.BASKET_CONFIRMED_TOPIC)) throw new ArgumentException(nameof(settings.Value.BASKET_CONFIRMED_TOPIC));

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = settings.Value.MESSAGE_BROKER_HOST,
                GroupId = "DeliveryConsumerGroup",
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _topic = settings.Value.BASKET_CONFIRMED_TOPIC;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(100), cancellationToken);
                    var consumeResult = _consumer.Consume(cancellationToken);

                    if (consumeResult.IsPartitionEOF) continue;

                    var basketConfirmedEvent =
                        JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(consumeResult.Message.Value);

                    var createOrderCommand = new CreateOrderCommand(
                        Guid.Parse(basketConfirmedEvent.BasketId),
                    basketConfirmedEvent.Address.Street);

                    var response = await _mediator.Send(createOrderCommand, cancellationToken);
                    if (response.IsFailure) Console.WriteLine(response.Error.Message);

                    try
                    {
                        _consumer.StoreOffset(consumeResult);
                    }
                    catch (KafkaException e)
                    {
                        Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
