using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using DeliveryApp.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class CourierRepository : ICourierRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CourierRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(Courier courier)
        {
            if (courier.Status != null) dbContext.Attach(courier.Status);
            if (courier.Transport != null) dbContext.Attach(courier.Transport);

            await dbContext.Couriers.AddAsync (courier);
        }

        public async Task<IEnumerable<Courier>> GetAllFreeAsync()
        {
            var couriers = await dbContext
                 .Couriers
                 .Include(c => c.Location)
                 .Include(c => c.Status)
                 .Include(c => c.Transport)
                 .Where(o => o.Status == CourierStatus.Free).ToListAsync();
            return couriers;
        }

        public async Task<Courier> GetByIdAsync(Guid id)
        {
            var courier =  await dbContext
             .Couriers
             .Include(c => c.Location)
             .Include(c => c.Status)
             .Include(c => c.Transport)
             .FirstOrDefaultAsync(o => o.Id == id);
            return courier;
        }

        public void Update(Courier courier)
        {
            if (courier.Status != null) dbContext.Attach(courier.Status);
            if (courier.Transport != null) dbContext.Attach(courier.Transport);

            dbContext.Couriers.Update(courier);
        }
    }
}
