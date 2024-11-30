using CSharpFunctionalExtensions;
using DeliveryApp.Core.SharedKernel;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Ports
{
    public interface IGeoService
    {
        Task<Result<Location,Error>> GetGeolocationAsync(string street, CancellationToken cancellationToken);
    }
}
