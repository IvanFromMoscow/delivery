using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.SharedKernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public class TransportShould
    {
        public static IEnumerable<object[]> TransportTestData()
        {
            yield return [Transport.Pedestrian, 1, "pedestrian", 1];
            yield return [Transport.Bicycle, 2, "bicycle", 2];
            yield return [Transport.Car, 3, "car", 3];
        }

        [Theory]
        [MemberData(nameof(TransportTestData))]
        public void HaveCorrectData(Transport transport, int id, string name, int speed)
        {
            //Arrange

            //Act
            
            //Assert
            transport.Name.Should().Be(name);
            transport.Id.Should().Be(id);
            transport.Speed.Should().Be(speed);
        }

        [Theory]
        [InlineData(1, "pedestrian", 1)]
        [InlineData(2, "bicycle", 2)]
        [InlineData(3, "car", 3)]
        public void CanBeFoundByName(int id, string name, int speed)
        {
            //Arrange

            //Act
            var result = Transport.FromName(name);
            //Assert
            result.Should().NotBeNull();
            result.Value.Id.Should().Be(id);
            result.Value.Name.Should().Be(name);
            result.Value.Speed.Should().Be(speed);
        }

        [Theory]
        [InlineData(1, "pedestrian", 1)]
        [InlineData(2, "bicycle", 2)]
        [InlineData(3, "car", 3)]
        public void CanNotBeFoundByName(int id, string name, int speed)
        {
            //Arrange
            string incorrectName = "test";
            //Act
            var result = Transport.FromName(incorrectName);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Theory]
        [InlineData(1, "pedestrian", 1)]
        [InlineData(2, "bicycle", 2)]
        [InlineData(3, "car", 3)]
        public void CanBeFoundById(int id, string name, int speed)
        {
            //Arrange

            //Act
            var result = Transport.FromId(id);
            //Assert
            result.Should().NotBeNull();
            result.Value.Id.Should().Be(id);
            result.Value.Name.Should().Be(name);
            result.Value.Speed.Should().Be(speed);
        }
        [Theory]
        [InlineData(1, "pedestrian", 1)]
        [InlineData(2, "bicycle", 2)]
        [InlineData(3, "car", 3)]
        public void CanNotBeFoundById(int id, string name, int speed)
        {
            //Arrange
            int incorrectId = -1;
            //Act
            var result = Transport.FromId(incorrectId);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void MatchTwoTransportsWithTheSameId()
        {
            //Arrange
            var first = Transport.Bicycle;
            var second = Transport.Bicycle;
            bool result = false;
            
            //Act
            result = first.Id == second.Id;
            //Assert
            result.Should().BeTrue();
            
            //Act
            result = first.Equals(second);
            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public void NotMatchTwoTransportsWithDifferentId()
        {
            //Arrange
            var first = Transport.Bicycle;
            var second = Transport.Car;
            bool result = false;

            //Act
            result = first.Id != second.Id;
            //Assert
            result.Should().BeTrue();

            //Act
            result = first.Equals(second);
            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnListWithTransports()
        {
            //Arrange

            //Act
            var transports = Transport.List();
            //Assert
            transports.Should().NotBeNull();
            transports.Should().AllBeOfType<Transport>();
            transports.Should().NotBeEmpty();
        }
    }
}
