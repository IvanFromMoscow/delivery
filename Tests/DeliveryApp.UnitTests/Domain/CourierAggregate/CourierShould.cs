using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.SharedKernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public class CourierShould
    {
        private Courier courier;

       
        public static IEnumerable<object[]> CourierWithNotCorrectData()
        {
            yield return [string.Empty, Transport.Bicycle, Location.Create(1,1).Value];
            yield return ["Jhon", null, Location.Create(1, 1).Value];
            yield return ["Jhon", Transport.Car, null];
            yield return [string.Empty, Transport.Car, null];
        }
        public static IEnumerable<object[]> CouriersForCheckMove()
        {
            yield return ["Jhon", Transport.Bicycle, Location.Create(1,1).Value, Location.Create(5,5).Value, Location.Create(3,1).Value];
            yield return ["Jhon", Transport.Bicycle, Location.Create(1,1).Value, Location.Create(1,1).Value, Location.Create(1,1).Value];
            yield return ["Jhon", Transport.Bicycle, Location.Create(5,5).Value, Location.Create(1,1).Value, Location.Create(3,5).Value];
            yield return ["Jhon", Transport.Bicycle, Location.Create(2,2).Value, Location.Create(1,5).Value, Location.Create(1,3).Value];
            yield return ["Jhon", Transport.Bicycle, Location.Create(1,5).Value, Location.Create(2,2).Value, Location.Create(2,4).Value];
            yield return ["Jhon", Transport.Car, Location.Create(1,5).Value, Location.Create(2,2).Value, Location.Create(2,3).Value];
            yield return ["Jhon", Transport.Car, Location.Create(2, 2).Value, Location.Create(1, 5).Value, Location.Create(1, 4).Value];

        }


        [Fact]
        public void BeCreateCourierWithCorrectParameters()
        {
            // Arrange
            var name = "Jhon";
            var transport = Transport.Bicycle;
            var location = Location.Create(1,1).Value;
            // Act
            var result = Courier.Create(name, transport, location);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Should().NotBeNull();
            result.Value.Name.Should().Be(name);
            (result.Value.Location == location).Should().BeTrue();
            (result.Value.Transport == transport).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(CourierWithNotCorrectData))]
        public void BeNotCreateCourierWithNotCorrectParameters(string name, Transport transport, Location location)
        {
            // Arrange
           
            // Act
            var result = Courier.Create(name, transport, location);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeReturnCorrectTimeToPoint()
        {
            // Arrange
            var name = "Jhon";
            var transport = Transport.Bicycle;
            var location = Location.Create(1, 1).Value;
            var courier = Courier.Create(name, transport, location).Value;
            var targetLocation = Location.Create(5, 5).Value;
            // Act
            var result = courier.CalculateTimeToPoint(targetLocation);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(4);
        }

        [Fact]
        public void ReturnErrorWhenParamsIsInCorrectOnCalculateTimeToPoint()
        {
            // Arrange
            var name = "Jhon";
            var transport = Transport.Bicycle;
            var location = Location.Create(1, 1).Value;
            var courier = Courier.Create(name, transport, location).Value;
            Location targetLocation = null;
            // Act
            var result = courier.CalculateTimeToPoint(targetLocation);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(CouriersForCheckMove))]
        public void BeCorrectMove(string name, 
            Transport transport, 
            Location locationCourier, 
            Location locationTarget, 
            Location locationIntermediantValue)
        {
            // Arrange
            var courier = Courier.Create(name, transport, locationCourier).Value;
            // Act
            var result = courier.Move(locationTarget);
            var result2 = courier.Location == locationIntermediantValue;
            // Assert
            result.IsSuccess.Should().BeTrue();
            result2.Should().BeTrue();
        }

        [Fact]
        public void BeSetCorrectBusyStatusForCourier()
        {
            // Arrange
            var courier = Courier.Create("Jhon", Transport.Bicycle, Location.Create(1, 1).Value).Value;
            // Act
            var result = courier.SetBusy();
            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void BeNotSetCorrectBusyStatusForCourier()
        {
            // Arrange
            var courier = Courier.Create("Jhon", Transport.Bicycle, Location.Create(1, 1).Value).Value;
            courier.SetBusy();
            // Act
            var result = courier.SetBusy();
            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void BeSetCorrectFreeStatusForCourier()
        {
            // Arrange
            var courier = Courier.Create("Jhon", Transport.Bicycle, Location.Create(1, 1).Value).Value;
            courier.SetBusy();
            // Act
            var result = courier.SetFree();
            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void BeNotSetCorrectFreeStatusForCourier()
        {
            // Arrange
            var courier = Courier.Create("Jhon", Transport.Bicycle, Location.Create(1, 1).Value).Value;
            // Act
            var result = courier.SetFree();
            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void BeCorrectCourierGetToTargetLocation()
        {
            // Arrange
            
            var courier = Courier.Create("Jhon", Transport.Bicycle, Location.Create(1, 1).Value).Value;
            var targetLocation = Location.Create(5, 5).Value;

            var expectedLocations = new List<Location> 
            { 
                Location.Create(3,1).Value,
                Location.Create(5,1).Value,
                Location.Create(5,3).Value,
                Location.Create(5,5).Value
            };

            var actualLocations = new List<Location>();
            // Act
            
            while (targetLocation != courier.Location) 
            {
                if (courier.Move(targetLocation).IsSuccess)
                {
                    actualLocations.Add(courier.Location);
                }
            }

            // Assert
            actualLocations.Should().BeEquivalentTo(expectedLocations);
        }
    }
}
