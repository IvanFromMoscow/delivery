using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Entities
{
    public sealed class OutboxMessage
    {
        /// <summary>
        /// Уникальный идентификатор сообщения
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Сообщение, которое требует отправки во внешний сервис
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Дата создания сообщения
        /// </summary>
        public DateTime CreatedDateUtc { get; set; }
        /// <summary>
        /// Дата публикации сообщения
        /// </summary>
        public DateTime? PublishedDateUtc { get; set; }
    }
}
