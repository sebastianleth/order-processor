using NodaTime;
using OrderProcessor.Domain;

namespace OrderProcessor.Handlers;

public abstract class HandlerFixture
{
    static Customer CustomerFactory(CustomerId id, CustomerState state) => new(id, state, SystemClock.Instance);

    protected const string CustomerEmail = "email@gmail.com";

    protected readonly Persistence.IAggregateRepository<CustomerId, Customer> Repository = new Persistence.InMemoryAggregateRepository<CustomerId, Customer, CustomerState>(CustomerFactory);
}