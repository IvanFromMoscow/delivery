using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.SharedKernel
{
    public class Location : ValueObject
    {
        /// <summary>
        /// Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Location()
        {

        }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="x">Значение параметра по горизонтали (ось X)</param>
        /// <param name="y">Значение параметра по вертикали (ось Y)</param>
        private Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Параметр по горизонтали (ось X)
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Параметр по вертикали (ось Y)
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="x">Значение параметра по горизонтали (ось X)</param>
        /// <param name="y">Значение параметра по вертикали (ось Y)</param>
        /// <returns></returns>
        public static Result<Location, Error> Create(int x, int y)
        {
            if (x < 1 || x > 10) return GeneralErrors.ValueIsInvalid(nameof(x));
            if (y < 1 || y > 10) return GeneralErrors.ValueIsInvalid(nameof(y));

            return new Location(x, y);
        }

        /// <summary>
        /// Расстояние до указанной координаты
        /// </summary>
        /// <param name="targetLocation">Конечная координата</param>
        /// <returns>Расстояние между координатами</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int DistanceTo(Location targetLocation)
        {
            if (targetLocation == null) throw new ArgumentNullException(nameof(targetLocation));

            var x = Math.Abs(targetLocation.X - this.X);
            var y = Math.Abs(targetLocation.Y - this.Y);

            return x + y;
        }
        /// <summary>
        /// Создает случайную координату
        /// </summary>
        /// <returns>Созданная координата</returns>
        public static Location CreateRandom()
        {
            var rnd = new Random();
            return new Location(rnd.Next(1, 11), rnd.Next(1, 11));
        }

        /// <summary>
        /// Перегрузка для определения идентичности
        /// </summary>
        /// <returns>Результат</returns>
        /// <remarks>Идентичность будет происходить по совокупности полей указанных в методе</remarks>
        [ExcludeFromCodeCoverage]
        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }
}
