using OrderProcessor.Domain;

namespace OrderProcessor.Handlers;

public class PlaceOrderHandler : ICommandHandler<Commands.PlaceOrder>
{
    readonly Persistence.IAggregateRepository<CustomerId, Customer> _repository;
    readonly Email.ISender _emailSender;

    public PlaceOrderHandler(Persistence.IAggregateRepository<CustomerId, Customer> repository, Email.ISender emailSender)
    {
        _repository = repository;
        _emailSender = emailSender;
    }

    public async Task Handle(Commands.PlaceOrder command)
    {
        var customerId = CustomerId.FromEmail(command.Email);
        var customer = await _repository.Load(customerId);

        var orderPlaced = customer.Handle(command);

        await _repository.Save(customer);

        var parameters = Email.Parameters.From(customer, orderPlaced);
        await _emailSender.SendEmail(parameters);
    }
}