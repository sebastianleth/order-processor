using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace OrderProcessor.Handlers
{
    public class CreateCustomerHandlerTests
    {
        readonly ICommandHandler<Commands.CreateCustomer> _sut;

        public CreateCustomerHandlerTests()
        {
            //_sut = new CreateCustomerHandler()
        }

        [Fact]
        public async Task GivenMessagingClient_WhenProcess_ThenTryDequeueTwice()
        {
        }

        [Fact]
        public async Task GivenMessagingClient_AndMessagesInQueue_WhenProcess_ThenInvokeHandlers()
        {

        }
    }
}
