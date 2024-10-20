﻿using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.SharedKernel;
using Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    public class Order : Aggregate
    {
        /// <summary>
        /// Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Order()
        {

        }
        private Order(Guid orderId, Location location)
        {
            Id = orderId;
            Location = location;
            Status = OrderStatus.Created;
        }

        /// <summary>
        /// Местоположение, куда нужно доставить заказ
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderStatus Status { get; private set; }

        /// <summary>
        /// Id назначенного курьера
        /// </summary>
        public Guid? CourierId { get; private set; }

        /// <summary>
        ///     Factory Method
        /// </summary>
        /// <param name="location">Местоположение, куда нужно доставить заказ</param>
        /// <returns>Результат</returns>
        public static Result<Order, Error> Create(Guid orderId, Location location)
        {
            if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
            return new Order(orderId, location);
        }

        /// <summary>
        /// Завершить заказ
        /// </summary>
        /// <returns>Результат</returns>
        public UnitResult<Error> Complete()
        {
            if (Status != OrderStatus.Assigned) return Errors.CompleteOrderIsWrong();
            
            Status = OrderStatus.Completed;
            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Назначить курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>Результат</returns>
        public UnitResult<Error> Assign(Courier courier)
        {
            if (courier == null) return GeneralErrors.ValueIsRequired(nameof(courier));
            if (courier.Status == CourierStatus.Busy) return Errors.CanNotAssignOrderForBusyCourier(courier.Id);
            
            Status = OrderStatus.Assigned;
            CourierId = courier.Id;
            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Ошибки, которые может возвращать сущность
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static class Errors
        {
            public static Error CompleteOrderIsWrong()
            {
                return new Error($"{nameof(Order).ToLowerInvariant()}.complete.order.is.wrong",
                        $"Завершить можно только назначенный ранее заказ.");
            }
            public static Error CanNotAssignOrderForBusyCourier(Guid courierId)
            {
                return new Error($"{nameof(Order).ToLowerInvariant()}.can.not.assign.order.for.busy.courier",
                        $"Нельзя назначить заказ курьеру c Id {courierId}, который находится в состоянии занят.");
            }
        }
    }
}
