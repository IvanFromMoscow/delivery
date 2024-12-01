using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Ports
{
    public interface INotificationProducer
    {
         Task Publish(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken);
    }
}
