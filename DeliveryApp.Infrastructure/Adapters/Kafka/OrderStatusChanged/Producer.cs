using Confluent.Kafka;
using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderStatusChanged;
using Primitives.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.OrderStatusChanged
{
    public class Producer : INotificationProducer
    {
        private readonly ProducerConfig config;
        private readonly string topicName;
        public Producer(IOptions<Settings> options)
        {
            if (string.IsNullOrWhiteSpace(options.Value.MESSAGE_BROKER_HOST)) throw new ArgumentException(nameof(options.Value.MESSAGE_BROKER_HOST));
            if (string.IsNullOrWhiteSpace(options.Value.ORDER_STATUS_CHANGED)) throw new ArgumentException(nameof(options.Value.ORDER_STATUS_CHANGED));

            config = new ProducerConfig
            {
                BootstrapServers = options.Value.MESSAGE_BROKER_HOST
            };
            topicName = options.Value.ORDER_STATUS_CHANGED;

        }
        public async Task Publish(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            // Перекладываем данные из Domain Event в Integration Event
            var orderStatusChangedIntegrationEvent = new OrderStatusChangedIntegrationEvent
            {
                OrderId = notification.orderId.ToString(),
                OrderStatus = GetOrderStatusFromOrderStatusChangedIntegrationEvent(notification.status),
            };

            // Создаем сообщение для Kafka
            var message = new Message<string, string>
            {
                Key = notification.EventId.ToString(),
                Value = JsonConvert.SerializeObject(orderStatusChangedIntegrationEvent)
            };

            try
            {
                // Отправляем сообщение в Kafka
                using var producer = new ProducerBuilder<string, string>(config).Build();
                var dr = await producer.ProduceAsync(topicName, message, cancellationToken);
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }

        }

        private global::OrderStatusChanged.OrderStatus GetOrderStatusFromOrderStatusChangedIntegrationEvent(Core.Domain.Model.OrderAggregate.OrderStatus status)
        {
            if (status == null) throw new ArgumentException(nameof(OrderStatus));
            return Core.Domain.Model.OrderAggregate.OrderStatus.List().Where(o => o.Name == status.Name)
                .Select(x => x.Name.ToEnum<global::OrderStatusChanged.OrderStatus>()).FirstOrDefault();
        }
    }
}
