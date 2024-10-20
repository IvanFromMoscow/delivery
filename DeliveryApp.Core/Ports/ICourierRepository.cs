using DeliveryApp.Core.Domain.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface ICourierRepository : IRepository<Courier>
    {
        /// <summary>
        /// Добавить курьера
        /// </summary>
        /// <param name="courier"></param>
        /// <returns></returns>
        Task AddAsync(Courier courier);

        /// <summary>
        /// Обновить курьера
        /// </summary>
        /// <param name="courier"></param>
        /// <returns></returns>
        void Update(Courier courier);
        
        /// <summary>
        /// Получить курьера по идентификатору
        /// </summary>
        /// <param name="id">идентификатор курьера</param>
        /// <returns></returns>
        Task<Courier> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить всех свободных курьеров
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Courier>> GetAllFreeAsync();
    }
}
