using Api.Attributes;
using Api.Controllers;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Queries.GetAllBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetAllCreatedAndAssignedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeliveryApp.Api.Adapters.Http
{
    public class DeliveryController : DefaultApiController
    {
        private IMediator _mediator;

        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async override Task<IActionResult> CreateOrder()
        {
            var orderId = Guid.NewGuid();
            var street = "Несуществующая";
            var createOrderCommand =
                new CreateOrderCommand(orderId, street);
            var response = await _mediator.Send(createOrderCommand);
            if (response.IsSuccess) return Ok();
            return Conflict();
        }

        public async override Task<IActionResult> GetCouriers()
        {
            var getAllBusyCouriersQuery = new GetAllBusyCouriersQuery();
            var response = await _mediator.Send(getAllBusyCouriersQuery);
            
            if (response == null) return NotFound();
            var model = response.Couriers.ToList();
            return Ok(model);
        }

        public override async Task<IActionResult> GetOrders()
        {
            var getAllCreatedAndAssignedOrdersQuery = new GetAllCreatedAndAssignedOrdersQuery();
            var response = await _mediator.Send(getAllCreatedAndAssignedOrdersQuery);

            if (response == null) return NotFound();
            var model = response.Orders.ToList();
            return Ok(model);
        }

        [HttpGet]
        [Route("test")]
        [ValidateModelState]
        [SwaggerOperation("Test")]
        public ActionResult<string> Test()
        {
            return Ok("Test!!!!");
        }
    }
}
