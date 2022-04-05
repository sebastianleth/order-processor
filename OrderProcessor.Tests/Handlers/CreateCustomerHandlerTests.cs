using System.Threading.Tasks;
using NodaTime;
using OrderProcessor.Domain;
using Shouldly;
using Xunit;

namespace OrderProcessor.Handlers
{
    public class CreateCustomerHandlerTests : HandlerFixture
    {
        readonly ICommandHandler<Commands.CreateCustomer> _sut;

        public CreateCustomerHandlerTests()
        {
            _sut = new CreateCustomerHandler(Repository);
        }

        [Fact]
        public async Task GivenCommand_WhenHandle_ThenCustomerCreated()
        {
            await _sut.Handle(new Commands.CreateCustomer(MessageId.New, CustomerEmail));

            var customer = await Repository.Load(CustomerId.FromEmail(CustomerEmail));

            customer.ShouldNotBeNull();
        }
    }
}
