using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IDispatchService dispatchService;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public AssignOrderToCourierHandler(IDispatchService dispatchService, IServiceScopeFactory serviceScopeFactory)
        {
            this.dispatchService = dispatchService;
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public async Task<Result<bool,Error>> Handle(AssignOrderToCourierCommand command, CancellationToken cancellationToken)
        {
            using (var scope = serviceScopeFactory.CreateAsyncScope())
            {
                // check
                var scopedServices = scope.ServiceProvider;
                var orderRepository = scopedServices.GetRequiredService<IOrderRepository>();
                var courierRepository = scopedServices.GetRequiredService<ICourierRepository>();
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

                return await scopedServices.GetRequiredService<IUnitOfWork>().SaveEntitiesAsync(cancellationToken);
            }
            
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
