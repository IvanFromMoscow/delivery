# delivery
My study project.
# DB
dotnet ef database update --context "DeliveryApp.Infrastructure.Adapters.Postgres.ApplicationDbContext"  --startup-project ./DeliveryApp.Api --connection "Server=localhost;Port=5232;User Id=postgres;Password=password;Database=delivery;"