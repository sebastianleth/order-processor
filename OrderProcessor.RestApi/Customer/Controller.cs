using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderProcessor.Customer
{
    [ApiController]
    [Route("customer")]
    public class Controller : ControllerBase
    {
        readonly Messaging.IClient _messagingClient;

        public Controller(Messaging.IClient messagingClient)
        {
            _messagingClient = messagingClient;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new customer with e-mail address as identifier")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public Task CreateCustomer(Commands.CreateCustomer command) => _messagingClient.Enqueue(command);


        [HttpPost("order")]
        [SwaggerOperation(Summary = "Place an order for a customer with a given e-mail address")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public Task PlaceOrder(Commands.PlaceOrder command) => _messagingClient.Enqueue(command);
    }
}