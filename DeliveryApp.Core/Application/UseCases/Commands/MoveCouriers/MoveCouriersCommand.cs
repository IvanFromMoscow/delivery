using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers
{
    public class MoveCouriersCommand : IRequest<bool>
    {
        /// <summary>
        /// Ctr
        /// </summary>
        public MoveCouriersCommand() { }
    }
}
