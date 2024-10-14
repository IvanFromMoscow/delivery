using DeliveryApp.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Ports
{
    public interface IGeoService
    {
        Task<Location> GetGeolocationAsync(string street, CancellationToken cancellationToken);
    }
}
