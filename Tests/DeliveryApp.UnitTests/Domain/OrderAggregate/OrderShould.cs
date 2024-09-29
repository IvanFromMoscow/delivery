using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.SharedKernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.OrderAggregate
{
    public class OrderShould
    {
      
        public static IEnumerable<object[]> GetNotCorrectParamsData()
        {
            yield return [Guid.Empty, Location.CreateRandom()];
            yield return [Guid.NewGuid(), null];
        }

        [Fact]
        public void BeCorrectWhenParamsIsCorrect()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var location = Location.Create(5, 5).Value;

            //Act
            var result = Order.Create(orderId, location);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.Location.Should().Be(location);
        }

        [Theory]
        [MemberData(nameof(GetNotCorrectParamsData))]
        public void ReturnValueIsRequiredErrorWhenParamsIsNotCorrect(Guid orderId, Location location)
        {
            //Arrange

            var result = Order.Create(orderId, location);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeCorrectAssignToCourier()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5).Value).Value;
            var courier = Courier.Create("Peter", Transport.Bicycle, Location.Create(1, 1).Value).Value;

            //Act
            var result = order.Assign(courier);

            //Assert
            result.IsSuccess.Should().BeTrue();
            order.CourierId.Should().Be(courier.Id);
            order.Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public void CanComplete()
        {
            //Arrange
            //Arrange
            var order = Order.Create(Guid.NewGuid(), Location.Create(5, 5).Value).Value;
            var courier = Courier.Create("Peter", Transport.Car, Location.Create(1, 1).Value).Value;

            order.Assign(courier);

            //Act
            var result = order.Complete();

            //Assert
            result.IsSuccess.Should().BeTrue();
            order.CourierId.Should().Be(courier.Id);
            order.Status.Should().Be(OrderStatus.Completed);
        }

    }
}
