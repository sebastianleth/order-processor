namespace OrderProcessor.Ordering
{
    public class Customer : Aggregates.Aggregate<CustomerState>
    {
        public static Customer Create(Aggregates.AggregateId id, object state) => new(new CustomerId(id.Value), (CustomerState)state);

        private Customer(CustomerId id, CustomerState state) : base(id, state)
        {
        }

        public void Handle(CreateCustomerCommand command)
        {
            EnsureDoesNotExist();
            ApplyState(State with {Email = command.Email});
        }

        public void Handle(PlaceOrderCommand command)
        {
            EnsureExists();
            ApplyState(State);
        }
    }
}
