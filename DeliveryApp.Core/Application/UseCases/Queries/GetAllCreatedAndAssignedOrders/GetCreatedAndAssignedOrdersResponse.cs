using DeliveryApp.Core.Domain.OrderAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCreatedAndAssignedOrders
{
    public class GetCreatedAndAssignedOrdersResponse
    {
        public GetCreatedAndAssignedOrdersResponse(List<Order> orders)
        {
            Orders.AddRange(orders);
        }

        public List<Order> Orders { get; set; } = new();
    }
    public class Order
    {
        /// <summary>
        ///     Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Геопозиция (X,Y)
        /// </summary>
        public Location Location { get; set; }
    }

    public class Location
    {
        /// <summary>
        ///     Горизонталь
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///     Вертикаль
        /// </summary>
        public int Y { get; set; }
    }
}
