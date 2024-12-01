using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Domain.OrderAggregate.DomainEvents
{
        public sealed record OrderStatusChangedDomainEvent(Guid orderId, OrderStatus status) : DomainEvent;
}
