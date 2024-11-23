using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<bool, Error>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IOrderRepository orderRepository;
        private readonly IGeoService geoService;

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="orderRepository"></param>
        public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IGeoService geoService)
        {
            this.unitOfWork = unitOfWork;
            this.orderRepository = orderRepository;
            this.geoService = geoService;
        }
        public async Task<Result<bool, Error>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            // check
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

            return await unitOfWork.SaveEntitiesAsync(cancellationToken);
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
