using DeliveryApp.Core.Ports;
using DeliveryApp.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Adapters.gRPC
{
    public class GeoService : IGeoService
    {
        public Task<Location> GetGeolocationAsync(string street, CancellationToken cancellationToken)
        {
            return  Task.Run<Location>(() => Location.Create(3, 3).Value);
        }
    }
}
