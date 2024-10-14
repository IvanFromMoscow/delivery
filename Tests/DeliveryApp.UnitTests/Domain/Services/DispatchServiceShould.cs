using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.SharedKernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services
{
    public class DispatchServiceShould
    {
        private readonly IDispatchService dispatchService;
        public static IEnumerable<object[]> DataFailedTest()
        {
            yield return [null, null];
            yield return [Order.Create(Guid.NewGuid(), Location.Create(1,1).Value).Value, null];
            yield return [null, new List<Courier>()
            {
                Courier.Create("Jack", Transport.Bicycle, Location.Create(1, 1).Value).Value
            }];
        }

        public DispatchServiceShould()
        {
            dispatchService = new DispatchService();
        }

        [Theory]
        [MemberData(nameof(DataFailedTest))]
        public void ReturnIsRequiredErrorWhenOneOfTheParametersIsNullOrAllAreNull(Order order, List<Courier> couriers)
        {
            // Arrange
            // Act
            var result = new DispatchService().Dispatch(order, couriers);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
        [Fact]
        public void ReturnIsErrorInvalidLengthWhenListOfCouriersIsEmpty()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value);
            var couriers = new List<Courier>();
            // Act
            var result = dispatchService.Dispatch(order.Value, couriers);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeCorrectFindFreeCourierWithMinTimeToLocation()
        {
            // Arrange
            var couriers = new List<Courier>()
            {
                Courier.Create("Kate", Transport.Bicycle, Location.Create(1,1).Value).Value,
                Courier.Create("Jack", Transport.Car, Location.Create(2,2).Value).Value,
                Courier.Create("Bob", Transport.Pedestrian, Location.Create(3,3).Value).Value,
            };
            var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5).Value).Value;
            // Act
            var result = dispatchService.Dispatch(order, couriers);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Jack");
        }

        [Fact]
        public void BeCorrectFindFreeCourierWhoIsLocatedInTheSamePlaceAsTheOrder()
        {
            // Arrange
            var couriers = new List<Courier>()
            {
                Courier.Create("Kate", Transport.Bicycle, Location.Create(1,1).Value).Value,
                Courier.Create("Jack", Transport.Car, Location.Create(2,2).Value).Value,
                Courier.Create("Bob", Transport.Pedestrian, Location.Create(3,3).Value).Value,
            };
            var order = Order.Create(Guid.NewGuid(), Location.Create(2, 2).Value).Value;
            // Act
            var result = dispatchService.Dispatch(order, couriers);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Jack");
        }

    }
}
