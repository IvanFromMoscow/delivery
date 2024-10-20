using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrderToCourier
{
    public class AssignOrderToCourierHandler : IRequestHandler<AssignOrderToCourierCommand, Result<bool, Error>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IOrderRepository orderRepository;
        private readonly ICourierRepository courierRepository;
        private readonly IDispatchService dispatchService;

        public AssignOrderToCourierHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository, IDispatchService dispatchService)
        {
            this.unitOfWork = unitOfWork;
            this.orderRepository = orderRepository;
            this.courierRepository = courierRepository;
            this.dispatchService = dispatchService;
        }
        public async Task<Result<bool,Error>> Handle(AssignOrderToCourierCommand command, CancellationToken cancellationToken)
        {
            var ordersCreated = await orderRepository.GetAllCreatedAsync();
            if (!ordersCreated.Any())
            {
                return Errors.NoOrdersWithStatusCreated();
            }
            var freeCouriers = await courierRepository.GetAllFreeAsync();
            if (!freeCouriers.Any())
            {
                return Errors.NoFreeCouriersForAssigning();
            }
            foreach (var order in ordersCreated)
            {
                var courier = dispatchService.Dispatch(order, freeCouriers.ToList());
                if (courier.IsFailure)
                {
                    return courier.Error;
                }
                var assignedOrder = order.Assign(courier.Value);
                if (assignedOrder.IsFailure)
                {
                    return assignedOrder.Error;
                }
                var courierSetBusy = courier.Value.SetBusy();
                if (courierSetBusy.IsFailure)
                {
                    return courierSetBusy.Error;
                }
                orderRepository.Update(order);
                courierRepository.Update(courier.Value);
            }
            return await unitOfWork.SaveEntitiesAsync(cancellationToken);
        }


        /// <summary>
        ///     Ошибки, которые может возвращать сущность
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static class Errors
        {
            public static Error NoOrdersWithStatusCreated()
            {
                return new Error($"{nameof(AssignOrderToCourierHandler).ToLowerInvariant()}.no.orders.with.status.created",
                    $"Нет заказов для назначения курьерам!");
            }
            public static Error NoFreeCouriersForAssigning()
            {
                return new Error($"{nameof(AssignOrderToCourierHandler).ToLowerInvariant()}.no.free.couriers.for.assigning",
                    $"Нет свободных курьеров для назначения заказа!");
            }
        }
    }
}
