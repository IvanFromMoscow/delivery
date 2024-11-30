using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<bool, Error>>
    {
        private readonly IGeoService geoService;
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="orderRepository"></param>
        public CreateOrderHandler(IGeoService geoService, IServiceScopeFactory serviceScopeFactory)
        {
            this.geoService = geoService;
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public async Task<Result<bool, Error>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            using (var scope = serviceScopeFactory.CreateAsyncScope())
            {
                // check
                var scopedServices = scope.ServiceProvider;
                var orderRepository = scopedServices.GetRequiredService<IOrderRepository>();
                var order = await orderRepository.GetByIdAsync(command.BasketId);
                
                if (order != null) return Errors.OderHasAlreadyBeenCreated(order);

                // create order
                 var location = await geoService.GetGeolocationAsync(command.Street, cancellationToken);
                if (location.IsFailure) return GeneralErrors.ValueIsInvalid(location.Error.Message);
                var newOrder = Order.Create(command.BasketId, location.Value);
                if (newOrder.IsFailure)
                {
                    return false;
                }
                await orderRepository.AddAsync(newOrder.Value);

                return await scopedServices.GetRequiredService<IUnitOfWork>().SaveEntitiesAsync(cancellationToken);
            }
           
            
        }

        /// <summary>
        ///     Ошибки, которые может возвращать сущность
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static class Errors
        {
            public static Error OderHasAlreadyBeenCreated(Order order)
            {
                return new Error($"{nameof(order).ToLowerInvariant()}.has.already.been.created",
                    $"Заказ с номером {order.Id} уже был создан ранее.");
            }
        }
    }
}
