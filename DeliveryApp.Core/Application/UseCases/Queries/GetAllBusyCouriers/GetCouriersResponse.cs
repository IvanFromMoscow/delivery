using DeliveryApp.Core.Domain.CourierAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllBusyCouriers
{
    public class GetCouriersResponse
    {
        public GetCouriersResponse(List<Courier> couriers)
        {
            Couriers.AddRange(couriers);
        }

        public List<Courier> Couriers { get; set; } = new();
    }
    public class Courier
    {
        /// <summary>
        ///     Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Геопозиция (X,Y)
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        ///     Вид транспорта
        /// </summary>
        public int TransportId { get; set; }
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
