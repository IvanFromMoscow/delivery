using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.SharedKernel;
using Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    public class Courier : Aggregate
    {
        /// <summary>
        /// Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Courier()
        {
            
        }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="name"></param>
        /// <param name="transport"></param>
        /// <param name="location"></param>
        private Courier(string name, Transport transport, Location location) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            Transport = transport;
            Location = location;
            Status = CourierStatus.Free;
        }

        /// <summary>
        ///  Имя курьера
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Транспорт курьера
        /// </summary>
        public Transport Transport { get; }

        /// <summary>
        /// Местоположение курьера
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Статус курьера
        /// </summary>
        public CourierStatus Status { get; private set; }

        /// <summary>
        ///     Получить статус по названию
        /// </summary>
        /// <param name="name">Название</param>
        /// <returns>Статус</returns>
        public static Result<Courier, Error> Create(string name, Transport transport, Location location)
        {
            if (string.IsNullOrWhiteSpace(name)) return GeneralErrors.ValueIsRequired(nameof(name));
            if (transport is null) return GeneralErrors.ValueIsRequired(nameof(transport));
            if (location is null) return GeneralErrors.ValueIsRequired(nameof(location));

            return new Courier(name, transport, location);
        }

        /// <summary>
        ///  Вычисление количества шагов, которые курьер затратит на путь до заказа
        /// </summary>
        /// <param name="location">Местоположение заказа</param>
        /// <returns>Количество шагов, которые курьер затратит на путь до заказа</returns>
        public Result<double, Error> CalculateTimeToPoint(Location location)
        {
            if (location is null) return GeneralErrors.ValueIsRequired(nameof(location));
            
            var distance = this.Location.DistanceTo(location);
            return (double) distance / Transport.Speed;
        }   

        /// <summary>
        /// Перемещение курьера за ход
        /// </summary>
        /// <param name="targetLocation">Местоположение заказа</param>
        /// <returns>Результат</returns>
        public UnitResult<Error> Move(Location targetLocation)
        {
            if (targetLocation is null) return GeneralErrors.ValueIsRequired(nameof(targetLocation));

            var distanceX = targetLocation.X - this.Location.X;
            var distanceY = targetLocation.Y - this.Location.Y;
            
            var x = this.Location.X;
            var y = this.Location.Y;

            var remainderSteps = Transport.Speed;  

            if (distanceX > 0)
            {
                if (distanceX >= remainderSteps)
                {
                    x += remainderSteps;
                    this.Location = Location.Create(x, this.Location.Y).Value;
                    return UnitResult.Success<Error>();
                }
                if (distanceX < remainderSteps)
                {
                    x += distanceX;
                    this.Location = Location.Create(x, this.Location.Y).Value;
                    remainderSteps -= distanceX;
                }
                if (this.Location == targetLocation) return UnitResult.Success<Error>();
            }
            else if (distanceX < 0)
            {
                if (Math.Abs(distanceX) >= remainderSteps)
                {
                    x -= remainderSteps;
                    this.Location = Location.Create(x, this.Location.Y).Value;
                    return UnitResult.Success<Error>();
                }
                if (Math.Abs(distanceX) < remainderSteps)
                {
                    x -= Math.Abs(distanceX);
                    this.Location = Location.Create(x, this.Location.Y).Value;
                    remainderSteps -= Math.Abs(distanceX);
                }
                if (this.Location == targetLocation) return UnitResult.Success<Error>();
            }

            if (distanceY > 0)
            {
                if (distanceY >= remainderSteps)
                {
                    y += remainderSteps;
                    this.Location = Location.Create(this.Location.X, y).Value;
                    return UnitResult.Success<Error>();
                }
                if (distanceY < remainderSteps)
                {
                    y += distanceY;
                    this.Location = Location.Create(this.Location.X, y).Value;
                }
                if (this.Location == targetLocation) return UnitResult.Success<Error>();
            }
            else if (distanceY < 0)
            {
                if (Math.Abs(distanceY) >= remainderSteps)
                {
                    y -= remainderSteps;
                    this.Location = Location.Create(this.Location.X, y).Value;
                    return UnitResult.Success<Error>();
                }
                else if (Math.Abs(distanceY) < remainderSteps)
                {
                    y -= Math.Abs(distanceY);
                    this.Location = Location.Create(this.Location.X, y).Value;
                }
                if (this.Location == targetLocation) return UnitResult.Success<Error>();
            }
            this.Location = Location.Create(x, y).Value;
            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Установить статус Free
        /// </summary>
        public UnitResult<Error> SetFree()
        {
            if (Status == CourierStatus.Free) return Errors.StatusIsFree();
            Status = CourierStatus.Free;
            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Установить статус Busy
        /// </summary>
        public UnitResult<Error> SetBusy()
        {
            if (Status == CourierStatus.Busy) return Errors.StatusIsBusy();
            
            Status = CourierStatus.Busy;
            return UnitResult.Success<Error>();
        }
        
        /// <summary>
        ///     Ошибки, которые может возвращать сущность
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static class Errors
        {
            public static Error StatusIsBusy()
            {
                return new Error($"{nameof(CourierStatus).ToLowerInvariant()}.status.is.busy",
                    $"Курьер занят, нельзя изменить статус на {nameof(CourierStatus.Busy)}");
            }
            public static Error StatusIsFree()
            {
                return new Error($"{nameof(CourierStatus).ToLowerInvariant()}.status.is.free",
                    $"Курьер уже свободен.");
            }
        }
    }
}
