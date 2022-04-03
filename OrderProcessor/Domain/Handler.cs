using OrderProcessor.Aggregates;

namespace OrderProcessor.Domain;

public class Handler
{
    readonly IAggregateRepository _repository;

    public Handler(IAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(Commands.CreateCustomer command)
    {
        var customerId = CustomerId.FromEmail(command.Email);
        var customer = await _repository.Load<CustomerId, Customer, CustomerState>(customerId, Customer.Create);
        
        customer.Handle(command);

        await _repository.Save<CustomerId, Customer, CustomerState>(customer);
    }

    public async Task Handle(Commands.PlaceOrder command)
    {
        var customerId = CustomerId.FromEmail(command.CustomerEmail);
        var customer = await _repository.Load<CustomerId, Customer, CustomerState>(customerId, Customer.Create);

        customer.Handle(command);

        await _repository.Save<CustomerId, Customer, CustomerState>(customer);
    }
}