using DeliveryApp.Core.SharedKernel;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel
{
    public class LocationShould
    {
        [Fact]
        public void BeCorrectWhenParamsIsCorrectOnCreated()
        {
            //Arrange

            //Act
            var location = Location.Create(1, 1);

            //Assert
            location.IsSuccess.Should().BeTrue();
            location.Value.X.Should().Be(1);
            location.Value.Y.Should().Be(1);
        }

        [Theory]
        [InlineData(0, 5)]
        [InlineData(-3, 10)]
        [InlineData(5, 0)]
        [InlineData(5, -2)]
        [InlineData(2, 11)]
        [InlineData(11, 2)]
        [InlineData(11, 11)]
        [InlineData(-3, -1)]
        [InlineData(0, 0)]

        public void ReturnErrorWhenParamsIsInCorrectOnCreated(int x, int y)
        {
            //Arrange

            //Act
            var location = Location.Create(x, y);

            //Assert
            location.IsSuccess.Should().BeFalse();
            location.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeEqualWhenAllPropertiesIsEqual()
        {
            //Arrange
            var first = Location.Create(5, 5).Value;
            var second = Location.Create(5, 5).Value;

            //Act
            var result = first == second;

            //Assert
            result.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(LocationTestData))]
        public void BeNotEqualWhenAllPropertiesIsEqual(Location first)
        {
            //Arrange
            var second = Location.Create(5, 5).Value;

            //Act
            var result = first != second;

            //Assert
            result.Should().BeTrue();
        }

        public static IEnumerable<object[]> LocationTestData()
        {
            yield return new[] { Location.Create(5, 1).Value };
            yield return new[] { Location.Create(1, 5).Value };
        }

        [Fact]
        public void BeCorrectCalculateDistanceBetweenLocations()
        {
            //Arrange
            var sourceLocation = Location.Create(2, 6).Value;
            var targetLocation = Location.Create(4, 9).Value;

            //Act
            var result = sourceLocation.DistanceTo(targetLocation);
            var reverseResult = targetLocation.DistanceTo(sourceLocation);
            //Assert
            result.Should().Be(5);
            reverseResult.Should().Be(5);
        }

        [Fact]
        public void ReturnErrorWhenCalculateDistanceBetweenLocationsAndParamsIsNotCorrect()
        {
            //Arrange
            var sourceLocation = Location.Create(2, 6).Value;
            Location targetLocation = null;

            //Act
            Action act = () => sourceLocation.DistanceTo(targetLocation);
            var result = Record.Exception(act);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ArgumentNullException>(result);
        }

        [Fact]
        public void BeCorrectCreateRandomLocation()
        {
            //Arrange

            //Act
            var createdLocation = Location.CreateRandom();

            //Assert
            Assert.NotNull(createdLocation);
            Assert.IsType<Location>(createdLocation);
            Assert.InRange(createdLocation.X, 1, 10);
            Assert.InRange(createdLocation.Y, 1, 10);
        }
    }
}
