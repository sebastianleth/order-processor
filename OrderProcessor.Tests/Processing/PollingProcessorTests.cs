using System.Threading;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using OrderProcessor.Domain;
using Shouldly;
using Xunit;

namespace OrderProcessor.Processing
{
    public class PollingProcessorTests
    {
        IProcessor? _sut;

        private void InitSut(
            Messaging.IClient messagingClient,
            Handlers.ICommandHandler<Commands.CreateCustomer> createCustomerHandler,
            Handlers.ICommandHandler<Commands.PlaceOrder> placeOrderHandler)
        {
            _sut = new PollingProcessor(messagingClient, createCustomerHandler, placeOrderHandler, Serilog.Log.Logger);
        }

        [Fact]
        public async Task GivenMessagingClient_WhenProcess_ThenTryDequeueTwice()
        {
            var messagingClientMock = new Mock<Messaging.IClient>();

            InitSut(
                messagingClientMock.Object,
                Mock.Of<Handlers.ICommandHandler<Commands.CreateCustomer>>(),
                Mock.Of<Handlers.ICommandHandler<Commands.PlaceOrder>>());

            await _sut!.Process(new CancellationToken(canceled: true));

            messagingClientMock
                .Verify(client => client.TryDequeue(out It.Ref<Messaging.Message?>.IsAny), Times.Exactly(2));
        }

        [Fact]
        public async Task GivenMessagingClient_AndMessagesInQueue_WhenProcess_ThenInvokeHandlers()
        {
            var createCustomerCommand = new Commands.CreateCustomer(MessageId.New, "");
            var placeOrderCommand = new Commands.PlaceOrder(MessageId.New, 10, "");

            var messagingClientMock = new Mock<Messaging.IClient>();
            messagingClientMock.Setup(client => client.TryDequeue(out createCustomerCommand)).Returns(Task.FromResult(true));
            messagingClientMock.Setup(client => client.TryDequeue(out placeOrderCommand)).Returns(Task.FromResult(true));

            var createCustomerHandlerMock = new Mock<Handlers.ICommandHandler<Commands.CreateCustomer>>();
            var placeOrderHandlerMock = new Mock<Handlers.ICommandHandler<Commands.PlaceOrder>>();

            InitSut(
                messagingClientMock.Object,
                createCustomerHandlerMock.Object,
                placeOrderHandlerMock.Object);

            await _sut!.Process(new CancellationToken(canceled: true));

            createCustomerHandlerMock.Verify(handler => handler.Handle(createCustomerCommand), Times.Once);
            placeOrderHandlerMock.Verify(handler => handler.Handle(placeOrderCommand), Times.Once);
        }
    }
}
