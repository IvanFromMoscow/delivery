using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using DeliveryApp.Core.SharedKernel;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Application
{
    public class MoveCouriersHandlerShould
    {
        private readonly IUnitOfWork unitOfWorkMock;
        private readonly IOrderRepository orderRepositoryMock;
        private readonly ICourierRepository courierRepositoryMock;

        private Courier TestCourier => Courier.Create("Bob", Transport.Bicycle, Location.Create(1, 1).Value).Value;
        public MoveCouriersHandlerShould()
        {
            unitOfWorkMock = Substitute.For<IUnitOfWork>();
            orderRepositoryMock = Substitute.For<IOrderRepository>();
            courierRepositoryMock = Substitute.For<ICourierRepository>();
        }

        [Fact]
        public async Task ReturnTrueWhenMoveCourierExecuteCorrectly()
        {
            // Arrange
            var orders = new List<Order>()
            {
                Order.Create(Guid.NewGuid(), Location.Create(5,5).Value).Value,
            };
            orders[0].Assign(TestCourier);
           

            orderRepositoryMock.GetAllAssignedAsync()
                .Returns(Task.FromResult(orders.AsEnumerable()));
            orderRepositoryMock.Update(Arg.Any<Order>());

            courierRepositoryMock.GetByIdAsync(Arg.Any<Guid>())
               .Returns(Task.FromResult(TestCourier));
            courierRepositoryMock.Update(Arg.Any<Courier>());

            unitOfWorkMock.SaveEntitiesAsync()
                .Returns(Task.FromResult(true));

            // Act
            var command = new MoveCouriersCommand();
            var handler = new MoveCouriersHandler(unitOfWorkMock, orderRepositoryMock, courierRepositoryMock);
            var result = await handler.Handle(command, new CancellationToken());

            // Assert
            result.Should().BeTrue();
            orderRepositoryMock.Received(2);
            courierRepositoryMock.Received(2);
            unitOfWorkMock.Received(1);
        }
        [Fact]
        public async Task ReturnFalseWhenAssignedOrdersIsEmpty()
        {
            // Arrange
            var orders = new List<Order>()
            {
            };
            orderRepositoryMock.GetAllAssignedAsync()
                .Returns(Task.FromResult(Enumerable.Empty<Order>()));

            courierRepositoryMock.GetByIdAsync(Arg.Any<Guid>())
               .Returns(Task.FromResult(TestCourier));

            unitOfWorkMock.SaveEntitiesAsync()
                .Returns(Task.FromResult(true));

            // Act
            var command = new MoveCouriersCommand();
            var handler = new MoveCouriersHandler(unitOfWorkMock, orderRepositoryMock, courierRepositoryMock);
            var result = await handler.Handle(command, new CancellationToken());

            // Assert
            result.Should().BeFalse();
            orderRepositoryMock.Received();
            courierRepositoryMock.Received(0);
            unitOfWorkMock.Received(0);
        }
    }
}
