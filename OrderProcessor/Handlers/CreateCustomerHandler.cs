using OrderProcessor.Domain;

namespace OrderProcessor.Handlers;

public class CreateCustomerHandler : ICommandHandler<Commands.CreateCustomer>
{
    readonly Persistence.IAggregateRepository _repository;

    public CreateCustomerHandler(Persistence.IAggregateRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Handle(Commands.CreateCustomer command)
    {
        var customerId = CustomerId.FromEmail(command.Email);
        var customer = Customer.New(customerId);

        customer.Handle(command);

        await _repository.Insert<CustomerId, Customer, CustomerState>(customer);
    }
}