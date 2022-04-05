using System.Threading.Tasks;
using OrderProcessor.Domain;
using Shouldly;
using Xunit;

namespace OrderProcessor.Handlers
{
    public class PlaceOrderHandlerTests : HandlerFixture
    {
        readonly ICommandHandler<Commands.PlaceOrder> _sut;

        public PlaceOrderHandlerTests()
        {
            // Create Customer as setup
            var createCustomerHandler = new CreateCustomerHandler(Repository);
            createCustomerHandler.Handle(new Commands.CreateCustomer(MessageId.New, CustomerEmail)).Wait();

            _sut = new PlaceOrderHandler(Repository, new Email.Sender(Serilog.Log.Logger), new Email.Composer());
        }

        [Fact]
        public async Task GivenCommand_WhenHandle_ThenOrderPlacedWithCorrectIdAndTotal()
        {
            var messageId = MessageId.New;
            var total = 100;

            await _sut.Handle(new Commands.PlaceOrder(messageId, total, CustomerEmail));

            var customer = await Repository.Load(CustomerId.FromEmail(CustomerEmail));

            var placedOrder = customer.State.Orders
                .ShouldHaveSingleItem();

            placedOrder.Id.ShouldBe(new OrderId(messageId.Value));
            placedOrder.Total.ShouldBe(total);
        }
    }
}
