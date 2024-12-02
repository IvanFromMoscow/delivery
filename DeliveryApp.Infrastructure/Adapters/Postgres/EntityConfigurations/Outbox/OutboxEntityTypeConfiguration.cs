using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.Outbox
{
    internal class OutboxEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("outbox");

            entityTypeBuilder
                .Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entityTypeBuilder
                .Property(entity => entity.Type)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("type")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.Message)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("message")
                .IsRequired();
            
            entityTypeBuilder
               .Property(entity => entity.CreatedDateUtc)
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("created_date_utc")
               .IsRequired();
      
            entityTypeBuilder
               .Property(entity => entity.PublishedDateUtc)
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("published_date_utc")
               .IsRequired(false);
      
        }
    }
}
