using Dapper;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using MediatR;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllBusyCouriers
{
    public class GetAllBusyCouriersHandler : IRequestHandler<GetAllBusyCouriersQuery, GetCouriersResponse>
    {
        private readonly string connectionString;

        public async Task<GetCouriersResponse> Handle(GetAllBusyCouriersQuery command, CancellationToken cancellationToken)
        {

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var result = await connection.QueryAsync<Courier>(
                @"SELECT c.id, c.name, c.location_x, c.location_y, c.transport_id
                  FROM public.couriers c
                  WHERE c.status_id = @statusId"
                , new { statusId = CourierStatus.Busy.Id });

            if (result.AsList().Count == 0)
                return null;

            return new GetCouriersResponse(MapCouriers(result));

        }

        public GetAllBusyCouriersHandler(string connectionString)
        {
            this.connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));

        }
        private List<Courier> MapCouriers(IEnumerable<Courier> couriersFromDb)
        {
            List<Courier> couriers = new();

            foreach (var courier in couriersFromDb)
            {
                var newCourier = new GetAllBusyCouriers.Courier()
                {
                    Id = courier.Id,
                    Location = new GetAllBusyCouriers.Location()
                    {
                        X = courier.Location.X,
                        Y = courier.Location.Y
                    },
                    Name = courier.Name,
                    TransportId = courier.TransportId
                };
                couriers.Add(newCourier);
            }
            return couriers;
        }




    }
}
