using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Courier> Couriers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ApplyConfiguration
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourierEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourierStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TransportEntityTypeConfiguration());

            // Seed
            modelBuilder.Entity<OrderStatus>(b =>
            {
                var allStatuses = OrderStatus.List();
                b.HasData(allStatuses.Select(c => new { c.Id, c.Name }));
            });

            modelBuilder.Entity<CourierStatus>(b =>
            {
                var allStatuses = CourierStatus.List();
                b.HasData(allStatuses.Select(c => new { c.Id, c.Name }));
            });

            modelBuilder.Entity<Transport>(b =>
            {
                var allTransports = Transport.List().ToList();
                b.HasData(allTransports.Select(c => new { c.Id, c.Name, c.Speed }));
            });
        }
    }

}
