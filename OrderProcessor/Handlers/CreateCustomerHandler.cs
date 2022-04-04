using OrderProcessor.Domain;

namespace OrderProcessor.Handlers;

public class CreateCustomerHandler : ICommandHandler<Commands.CreateCustomer>
{
    readonly Persistence.IAggregateRepository<CustomerId, Customer> _repository;

    public CreateCustomerHandler(Persistence.IAggregateRepository<CustomerId, Customer> repository)
    {
        _repository = repository;
    }
    
    public async Task Handle(Commands.CreateCustomer command)
    {
        var customerId = CustomerId.FromEmail(command.Email);
        var customer = await _repository.New(customerId);

        customer.Handle(command);

        await _repository.Insert(customer);
    }
}