
using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator mediator;
        private bool _disposed;

        public UnitOfWork(ApplicationDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) _dbContext.Dispose();
                _disposed = true;
            }
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await PublishDomainEventsAsync();
            return true;

        }

        private async Task PublishDomainEventsAsync()
        {
            // агрегаты с доменными событиями
            var domainEntities = _dbContext.ChangeTracker
                .Entries<Aggregate>()
                .Where(x => x.Entity.GetDomainEvents().Any());
            
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.GetDomainEvents())
                .ToList();
            
            // Очистили доменные события в агрегаторе
            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            // отправляем в медиатр
            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }

}
