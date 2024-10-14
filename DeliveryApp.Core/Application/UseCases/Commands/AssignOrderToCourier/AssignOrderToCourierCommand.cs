using CSharpFunctionalExtensions;
using MediatR;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrderToCourier
{
    public class AssignOrderToCourierCommand : IRequest<Result<bool, Error>>
    {
        public AssignOrderToCourierCommand()
        {
            
        }
    }
}
