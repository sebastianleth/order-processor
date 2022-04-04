using OrderProcessor.Domain;

namespace OrderProcessor.Handlers;

public class PlaceOrderHandler : ICommandHandler<Commands.PlaceOrder>
{
    readonly Persistence.IAggregateRepository _repository;
    readonly Email.ISender _emailSender;

    public PlaceOrderHandler(Persistence.IAggregateRepository repository, Email.ISender emailSender)
    {
        _repository = repository;
        _emailSender = emailSender;
    }

    public async Task Handle(Commands.PlaceOrder command)
    {
        var customerId = CustomerId.FromEmail(command.CustomerEmail);
        var customer = await _repository.Load<CustomerId, Customer, CustomerState>(customerId, Customer.Initialize);

        customer.Handle(command);

        await _repository.Save<CustomerId, Customer, CustomerState>(customer);
        await _emailSender.SendEmail("Order placed!");
    }
}