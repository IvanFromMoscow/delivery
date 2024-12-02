
using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using MediatR;
using Newtonsoft.Json;
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
            await SaveDomainEventsInOutboxAsync();
            await _dbContext.SaveChangesAsync(cancellationToken);
            //await PublishDomainEventsAsync();
            return true;

        }

        private async Task SaveDomainEventsInOutboxAsync()
        {
            var outboxMessages = _dbContext.ChangeTracker
           .Entries<Aggregate>() // Получили агрегаты в которых есть доменные события
           .Select(x => x.Entity)
           .SelectMany(aggregate =>
           {
               // Переложили в отдельную переменную
               var domainEvents = aggregate.GetDomainEvents();

               // Очистили Domain Event в самих агрегатах (поскольку далее они будут отправлены и больше не нужны)
               aggregate.ClearDomainEvents();
               return domainEvents;
           }
           )
           .Select(domainEvent => new OutboxMessage
           {
               // Создали объект OutboxMessage на основе Domain Event
               Id = domainEvent.EventId,
               CreatedDateUtc = DateTime.UtcNow,
               Type = domainEvent.GetType().Name,
               Message = JsonConvert.SerializeObject(
                   domainEvent,
                   new JsonSerializerSettings
                   {
                       // Эта настройка нужна, чтобы сериализовать Domain Event с указанием типов
                       // Если ее не указать, то десеарилизатор не поймет в какой тип восстанавоивать сообщение
                       TypeNameHandling = TypeNameHandling.All
                   })
           })
           .ToList();

            // Добавяляем OutboxMessages в dbContext
            // После выполнения этой строки в DbContext будут находится сам Aggregate и OutboxMessages
            await _dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages);
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
