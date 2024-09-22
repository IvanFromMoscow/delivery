﻿using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    public class Transport : Entity<int>
    {
        public static readonly Transport Pedestrian = new(1, nameof(Pedestrian).ToLowerInvariant(), 1);
        public static readonly Transport Bicycle = new(2, nameof(Bicycle).ToLowerInvariant(), 2);
        public static readonly Transport Car = new(3, nameof(Car).ToLowerInvariant(), 3);

        /// <summary>
        /// Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Transport()
        {

        }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        /// <param name="speed">Скорость</param>
        private Transport(int id, string name, int speed) : this()
        {
            Id = id;
            Name = name;
            Speed = speed;
        }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Скорость
        /// </summary>
        public int Speed { get; }


        /// <summary>
        /// Список всех значений списка
        /// </summary>
        /// <returns>Значения списка</returns>
        public static IEnumerable<Transport> List()
        {
            yield return Pedestrian;
            yield return Bicycle;
            yield return Car;
        }

        /// <summary>
        /// Получить транспорт по названию
        /// </summary>
        /// <param name="name">Название</param>
        /// <returns>Транспорт</returns>
        public static Result<Transport, Error> FromName(string name)
        {
            var transport = List().SingleOrDefault(t => t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (transport == null)
            {
                return Errors.TransportIsWrong();
            }
            return transport;
        }

        /// <summary>
        /// Получить транспорт по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Транспорт</returns>
        public static Result<Transport, Error> FromId(int id)
        {
            var transport = List().SingleOrDefault(t => t.Id == id);
            if (transport == null)
            {
                return Errors.TransportIsWrong();
            }
            return transport;
        }

        /// <summary>
        ///     Ошибки, которые может возвращать сущность
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static class Errors
        {
            public static Error TransportIsWrong()
            {
                return new Error($"{nameof(Transport).ToLowerInvariant()} is wrong",
                        $"Не верное значение. Допустимые значения: " +
                        $"{nameof(Transport).ToLowerInvariant()}: " +
                        $"{string.Join(",", List().Select(s => s.Name))}"
                    );
            }
        }
    }
}
