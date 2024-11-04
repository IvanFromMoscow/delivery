using Dapper;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using MediatR;
using Npgsql;

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
            var sql = $@"SELECT o.id, o.location_x as {nameof(Order.Location.X)} , o.location_y {nameof(Order.Location.Y)}
                  FROM public.orders o
                  WHERE o.status_id IN (@statusCreated, @statusAssigned)";

            var result = await connection.QueryAsync<Order,Location,Order>(
                sql, (order, location) =>
                {
                    order.Location = location;
                    return order;
                }, new { statusCreated = OrderStatus.Created.Id, statusAssigned = OrderStatus.Assigned.Id }
                ,splitOn: "X");

            if (result.AsList().Count == 0)
                return null;

            return new GetCreatedAndAssignedOrdersResponse(MapOrders(result));
        }

        private List<Order> MapOrders(IEnumerable<Order> ordersFromDb)
        {
            List<Order> orders = new();

            foreach (var order in ordersFromDb)
            {
                var newOrder = new GetAllCreatedAndAssignedOrders.Order()
                {
                    Id = order.Id,
                    Location = new GetAllCreatedAndAssignedOrders.Location()
                    {
                        X = order.Location.X,
                        Y = order.Location.Y
                    }
                };
                orders.Add(newOrder);
            }
            return orders;
        }
    }
}
