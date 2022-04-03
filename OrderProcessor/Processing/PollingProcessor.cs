﻿
namespace OrderProcessor.Processing;

class PollingProcessor : IProcessor
{
    readonly Messaging.IClient _messagingClient;
    readonly Handlers.ICommandHandler<Commands.CreateCustomer> _createCustomerHandler;
    readonly Handlers.ICommandHandler<Commands.PlaceOrder> _placeOrderHandler;
    readonly TimeSpan _pollingInterval = TimeSpan.FromMilliseconds(100);

    public PollingProcessor(
        Messaging.IClient messagingClient,
        Handlers.ICommandHandler<Commands.CreateCustomer> createCustomerHandler,
        Handlers.ICommandHandler<Commands.PlaceOrder> placeOrderHandler)
    {
        _messagingClient = messagingClient;
        _createCustomerHandler = createCustomerHandler;
        _placeOrderHandler = placeOrderHandler;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        while (true)
        {
            // Need generic approach for reading all types of messages, instead of this per-type-basis
            // ... and an acknowledged queue, so that the message is retried in case of failure
            // ... and a proper listening queue, instead of this polling

            await HandleCreateCustomer();
            await HandlePlaceOrder();

            await Task.Delay(_pollingInterval, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    async Task HandleCreateCustomer()
    {
        var gotMessage = await _messagingClient.TryDequeue<Commands.CreateCustomer>(out var command);

        if (gotMessage)
        {
            await _createCustomerHandler.Handle(command!);
        }
    }

    async Task HandlePlaceOrder()
    {
        var gotMessage = await _messagingClient.TryDequeue<Commands.PlaceOrder>(out var command);

        if (gotMessage)
        {
            await _placeOrderHandler.Handle(command!);
        }
    }
}