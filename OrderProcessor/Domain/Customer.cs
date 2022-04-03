namespace OrderProcessor.Domain
{
    public class Customer : Aggregates.Aggregate<CustomerState>
    {
        public static Customer Create(CustomerId id, CustomerState state) => new(id, state);

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
