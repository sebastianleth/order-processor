using Microsoft.AspNetCore.Mvc;

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
        [Consumes("application/json")]
        [Produces("application/json")]
        public Task CreateCustomer(Commands.CreateCustomer command) => _messagingClient.Enqueue(command);


        [HttpPost("order")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public Task PlaceOrder(Commands.PlaceOrder command) => _messagingClient.Enqueue(command);
    }
}