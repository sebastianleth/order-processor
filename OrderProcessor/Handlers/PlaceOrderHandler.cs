using OrderProcessor.Domain;

namespace OrderProcessor.Handlers;

public class PlaceOrderHandler : ICommandHandler<Commands.PlaceOrder>
{
    readonly Persistence.IAggregateRepository<CustomerId, Customer> _repository;
    readonly Email.ISender _emailSender;
    readonly Email.IComposer _emailComposer;

    public PlaceOrderHandler(
        Persistence.IAggregateRepository<CustomerId, Customer> repository, 
        Email.ISender emailSender,
        Email.IComposer emailComposer)
    {
        _repository = repository;
        _emailSender = emailSender;
        _emailComposer = emailComposer;
    }

    public async Task Handle(Commands.PlaceOrder command)
    {
        var customerId = CustomerId.FromEmail(command.Email);
        var customer = await _repository.Load(customerId);

        var orderPlaced = customer.PlaceOrder(command);

        await _repository.Save(customer);

        var parameters = Email.Parameters.From(customer, orderPlaced);
        var email = _emailComposer.Do(parameters);
        await _emailSender.SendEmail(email);
    }
}