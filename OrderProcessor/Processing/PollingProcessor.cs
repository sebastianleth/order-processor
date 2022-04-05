namespace OrderProcessor.Processing;

class PollingProcessor : IProcessor
{
    readonly Messaging.IClient _messagingClient;
    readonly Handlers.ICommandHandler<Commands.CreateCustomer> _createCustomerHandler;
    readonly Handlers.ICommandHandler<Commands.PlaceOrder> _placeOrderHandler;
    readonly Serilog.ILogger _logger;
    readonly TimeSpan _pollingInterval = TimeSpan.FromMilliseconds(100);

    public PollingProcessor(
        Messaging.IClient messagingClient,
        Handlers.ICommandHandler<Commands.CreateCustomer> createCustomerHandler,
        Handlers.ICommandHandler<Commands.PlaceOrder> placeOrderHandler, 
        Serilog.ILogger logger
        )
    {
        _messagingClient = messagingClient;
        _createCustomerHandler = createCustomerHandler;
        _placeOrderHandler = placeOrderHandler;
        _logger = logger;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                await HandleCreateCustomer();
                await HandlePlaceOrder();
                await DelayNoThrow(cancellationToken);
            }
            catch (Exception e)
            {
                LogError(e);
            }

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

    void LogError(Exception e)
    {
        _logger.Error(e, "Unhandled exception in PollingProcessor");
    }

    Task DelayNoThrow(CancellationToken cancellationToken)
    {
        try
        {
            return Task.Delay(_pollingInterval, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            return Task.CompletedTask;
        }
    }
}