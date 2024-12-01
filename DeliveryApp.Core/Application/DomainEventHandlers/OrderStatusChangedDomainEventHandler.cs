using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.DomainEventHandlers
{
    public class OrderStatusChangedDomainEventHandler : INotificationHandler<OrderStatusChangedDomainEvent>
    {
        private readonly INotificationProducer notificationProducer;

        public OrderStatusChangedDomainEventHandler(INotificationProducer notificationProducer)
        {
            this.notificationProducer = notificationProducer;
        }
        public async Task Handle(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            await notificationProducer.Publish(notification, cancellationToken);
        }
    }
}
