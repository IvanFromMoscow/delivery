using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Primitives;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Adapters.Postgres.Repositories
{
    public class CourierRepositoryTestsShould : IAsyncLifetime
    {
        /// <summary>
        ///     Настройка Postgres из библиотеки TestContainers
        /// </summary>
        /// <remarks>По сути это Docker контейнер с Postgres</remarks>
        private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("couriers")
            .WithUsername("username")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();
        private readonly Courier courier;
        private ApplicationDbContext dbContext;

        /// <summary>
        /// Ctr
        /// </summary>
        public CourierRepositoryTestsShould()
        {
            courier = Courier.Create("Jhon", Transport.Pedestrian, Location.Create(1, 1).Value).Value;
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
        public async void CanAddCourier()
        {
            //Arrange

            //Act
            var courierRepository = new CourierRepository(dbContext);
            await courierRepository.AddAsync(courier);
            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var curierFromDb = await courierRepository.GetByIdAsync(courier.Id);
            courier.Should().BeEquivalentTo(curierFromDb);

        }

        [Fact]
        public async Task CanUpdateCourier()
        {
            //Arrange
            var courierRepository = new CourierRepository(dbContext);
            await courierRepository.AddAsync(courier);
            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var courierFromDb = await courierRepository.GetByIdAsync(courier.Id);
            courierFromDb.SetBusy();
            courierRepository.Update(courierFromDb);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var courierFromDbAssert = await courierRepository.GetByIdAsync(courierFromDb.Id);
            courierFromDb.Should().BeEquivalentTo(courierFromDbAssert);
            courierFromDbAssert.Status.Should().Be(courierFromDb.Status);
        }

        [Fact]
        public async Task CanGetByCourierId()
        {
            //Arrange
            var courierRepository = new CourierRepository(dbContext);
            await courierRepository.AddAsync(courier);
            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var courierFromDb = await courierRepository.GetByIdAsync(courier.Id);

            //Assert
            courier.Should().BeEquivalentTo(courierFromDb);
        }


        [Fact]
        public async Task CanGetAllFreeCouriers()
        {
            //Arrange
            var courierTwo = Courier.Create("Jack", Transport.Car, Location.Create(2,2).Value).Value;
            courierTwo.SetBusy();
            var courierRepository = new CourierRepository(dbContext);

            await courierRepository.AddAsync(courier);
            await courierRepository.AddAsync(courierTwo);

            var unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var couriers = courierRepository.GetAllFree()?.ToList();

            //Assert
            couriers.Should().NotBeEmpty();
            couriers.Count().Should().Be(1);
            couriers.Single().Should().BeEquivalentTo(courier);
        }

    }
}
