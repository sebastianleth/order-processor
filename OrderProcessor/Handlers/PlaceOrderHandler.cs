using OrderProcessor.Domain;
using OrderProcessor.Email;

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
        var customerId = CustomerId.FromEmail(command.Email);
        var customer = await _repository.Load<CustomerId, Customer, CustomerState>(customerId, Customer.Initialize);

        var orderPlaced = customer.Handle(command);

        await _repository.Save<CustomerId, Customer, CustomerState>(customer);

        var parameters = EmailParameters.From(customer, orderPlaced);
        await _emailSender.SendEmail(parameters);
    }

    
}