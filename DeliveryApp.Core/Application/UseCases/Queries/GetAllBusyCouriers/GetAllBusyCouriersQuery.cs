﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllBusyCouriers
{
    public class GetAllBusyCouriersQuery : IRequest<GetCouriersResponse>
    {
    }
}
