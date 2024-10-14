using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Adapters.Postgres.Repositories
{
    public class OrderRepositoryTestShould : IAsyncLifetime
    {
        /// <summary>
        ///     Настройка Postgres из библиотеки TestContainers
        /// </summary>
        /// <remarks>По сути это Docker контейнер с Postgres</remarks>
        private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("orders")
            .WithUsername("username")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();
        private readonly Order order;
        private ApplicationDbContext dbContext;

        public OrderRepositoryTestShould()
        {
            order = Order.Create(Guid.NewGuid(), Location.Create(1,1).Value).Value;
        }
        public async Task DisposeAsync()
        {
            await postgreSqlContainer.DisposeAsync().AsTask();
        }

        public async Task InitializeAsync()
        {
            await postgreSqlContainer.StartAsync();

            //Накатываем миграции и справочники
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); }).Options;
            dbContext = new ApplicationDbContext(contextOptions);
            dbContext.Database.Migrate();
        }

        [Fact]
        public async Task CanAddOrder()
        {
            //Arrange

            //Act
            var orderRepository = new OrderRepository(dbContext);
            await orderRepository.AddAsync(order);
            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var orderFromDb = await orderRepository.GetByIdAsync(order.Id);
            order.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async Task CanUpdateOrder()
        {
            //Arrange
            var orderRepository = new OrderRepository(dbContext);
            await orderRepository.AddAsync(order);
            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var orderFromDb = await orderRepository.GetByIdAsync(order.Id);
            orderFromDb.Complete();
            orderRepository.Update(orderFromDb);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var orderFromDbAssert = await orderRepository.GetByIdAsync(orderFromDb.Id);
            orderFromDb.Should().BeEquivalentTo(orderFromDbAssert);
            orderFromDbAssert.Status.Should().Be(orderFromDb.Status);
        }

        [Fact]
        public async Task CanGetByOrderId()
        {
            //Arrange
            var orderRepository = new OrderRepository(dbContext);
            await orderRepository.AddAsync(order);
            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var orderFromDb = await orderRepository.GetByIdAsync(order.Id);

            //Assert
            order.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async Task CanGetAllCreatedOrders()
        {
            //Arrange
            var orderTwo = Order.Create(Guid.NewGuid(), Location.Create(2, 2).Value).Value;
            var orderRepository = new OrderRepository(dbContext);

            await orderRepository.AddAsync(order);
            await orderRepository.AddAsync(orderTwo);

            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var orders = await orderRepository.GetAllCreatedAsync();

            //Assert
            orders.Should().NotBeEmpty();
            orders.Count().Should().Be(2);
            Assert.Contains(orders, o => o.Id.Equals(order.Id));
            Assert.Contains(orders, o => o.Id.Equals(orderTwo.Id));
        }

        [Fact]
        public async Task CanGetAllAssignedOrders()
        {
            //Arrange
            order.Assign(Courier.Create("Jack", Transport.Bicycle, Location.Create(4, 4).Value).Value);
            var orderTwo = Order.Create(Guid.NewGuid(), Location.Create(2, 2).Value).Value;
            orderTwo.Assign(Courier.Create("Jhon", Transport.Car, Location.Create(3, 3).Value).Value);
            
            var orderRepository = new OrderRepository(dbContext);

            await orderRepository.AddAsync(order);
            await orderRepository.AddAsync(orderTwo);

            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var orders = await orderRepository.GetAllAssignedAsync();

            //Assert
            orders.Should().NotBeEmpty();
            orders.Count().Should().Be(2);
            Assert.Contains(orders, o => o.Id.Equals(order.Id));
            Assert.Contains(orders, o => o.Id.Equals(orderTwo.Id));
        }
    }
}
