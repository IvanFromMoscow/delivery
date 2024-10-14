using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers
{
    public class MoveCouriersHandler : IRequestHandler<MoveCouriersCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IOrderRepository orderRepository;
        private readonly ICourierRepository courierRepository;

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="orderRepository"></param>
        public MoveCouriersHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository)
        {
            this.unitOfWork = unitOfWork;
            this.orderRepository = orderRepository;
            this.courierRepository = courierRepository;
        }
        public async Task<bool> Handle(MoveCouriersCommand command, CancellationToken cancellationToken)
        {
            var assignedOrders = await orderRepository.GetAllAssignedAsync();
            if (assignedOrders.Count() == 0)
            {
                return false;
            }
            foreach (var order in assignedOrders)
            {
                var courier = await courierRepository.GetByIdAsync(order.CourierId.Value);
                if (courier != null)
                {
                    var isMoved = courier.Move(order.Location);
                    if (isMoved.IsSuccess && courier.Location == order.Location) 
                    {
                        courier.SetFree();
                        order.Complete(); 
                    }
                }
                orderRepository.Update(order);
                courierRepository.Update(courier);
            }
            return await unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
