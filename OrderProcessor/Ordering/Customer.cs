namespace OrderProcessor.Ordering
{
    public class Customer : Aggregates.Aggregate<CustomerState>
    {
        public static Customer Create(Aggregates.AggregateId id, object state) => new(id, (CustomerState)state);

        private Customer(Aggregates.AggregateId id, CustomerState state) : base(id, state)
        {
        }

        public void Handle(CreateCustomerCommand command)
        {
            EnsureDoesNotExist();
        }

        public void Handle(PlaceOrderCommand command)
        {
            EnsureExists();
        }
    }
}
