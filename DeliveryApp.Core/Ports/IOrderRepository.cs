using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Добавить заказ
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task AddAsync(Order order);

        /// <summary>
        /// Обновить заказ
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        void Update(Order order);

        /// <summary>
        /// Получить заказ по идентификатору
        /// </summary>
        /// <param name="id">идентификатор заказа</param>
        /// <returns></returns>
        Task<Order> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить все новые заказы
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetAllCreatedAsync();

        /// <summary>
        /// Получить все назначенные заказы
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetAllAssignedAsync();
    }
}
