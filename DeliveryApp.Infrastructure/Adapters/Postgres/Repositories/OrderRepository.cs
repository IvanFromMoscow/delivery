using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OrderRepository(ApplicationDbContext context)
        {
            this.dbContext = context ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task AddAsync(Order order)
        {
            if (order.Status != null) dbContext.Attach(order.Status);

            await dbContext.Orders.AddAsync(order);
        }

        public void Update(Order order)
        {
            if (order.Status != null) dbContext.Attach(order.Status);

            dbContext.Orders.Update(order);
        }

        public async Task<IEnumerable<Order>> GetAllAssignedAsync()
        {
            return await dbContext
                .Orders
                .Include(o => o.Location)
                .Include(o => o.Status)
                .Where(o => o.Status == OrderStatus.Assigned).ToListAsync();

        }

        public async Task<IEnumerable<Order>> GetAllCreatedAsync()
        {
            return await dbContext
                .Orders
                .Include(o => o.Location)
                .Include(o => o.Status)
                .Where(o => o.Status == OrderStatus.Created).ToListAsync();
           
        }

        public async Task<Order> GetByIdAsync(Guid id)
        {
            var order = await dbContext
                .Orders
                .Include(o => o.Location)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.Id == id);
            return order;
        }

       
    }
}
