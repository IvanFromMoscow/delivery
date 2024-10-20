using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Domain.Services
{
    public class DispatchService : IDispatchService
    {
        public Result<Courier,Error> Dispatch(Order order, List<Courier> couriers)
        {
            if (order == null)  return GeneralErrors.ValueIsRequired(nameof(order));
            if (couriers == null) return GeneralErrors.ValueIsRequired(nameof(couriers));
            if (couriers.Count == 0) return GeneralErrors.InvalidLength(nameof(couriers));

            double minTime = Double.MaxValue;
            Courier findedCourier = null;

            foreach(var courier in couriers)
            {
                if (courier.Location.Equals(order.Location))
                {
                    return courier;
                }
                var time = courier.CalculateTimeToPoint(order.Location);
                if (time.IsSuccess && minTime > time.Value)
                {
                    minTime = time.Value;
                    findedCourier = courier;
                }
            }
            return findedCourier;
        }
    }
}
