using Dapper;
using DeliveryApp.Core.Application.UseCases.Queries.GetAllBusyCouriers;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using MediatR;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCreatedAndAssignedOrders
{
    public class GetAllCreatedAndAssignedOrdersHandler : IRequestHandler<GetAllCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>
    {
        private readonly string connectionString;

        public GetAllCreatedAndAssignedOrdersHandler(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString) ?
                connectionString :
                throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<GetCreatedAndAssignedOrdersResponse> Handle(GetAllCreatedAndAssignedOrdersQuery request, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var result = await connection.QueryAsync<dynamic>(
                @"SELECT o.id, o.location_x, o.location_y 
                  FROM public.orders o
                  WHERE o.status_id IN (@statusCreated, @statusAssigned)"
                , new { statusCreated = OrderStatus.Created.Id, statusAssigned = OrderStatus.Assigned.Id });

            if (result.AsList().Count == 0)
                return null;

            return new GetCreatedAndAssignedOrdersResponse(MapOrders(result));
        }

        private List<Order> MapOrders(IEnumerable<dynamic> ordersFromDb)
        {
            List<Order> orders = new();

            foreach (var order in ordersFromDb)
            {
                var newOrder = new GetAllCreatedAndAssignedOrders.Order()
                {
                    Id = order.id,
                    Location = new GetAllCreatedAndAssignedOrders.Location()
                    {
                        X = order.location_x,
                        Y = order.location_y
                    }
                };
                orders.Add(newOrder);
            }
            return orders;
        }
    }
}
