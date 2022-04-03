using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain
{
    public class Customer : Aggregates.Aggregate<CustomerState>
    {
        public static Customer Create(CustomerId id, CustomerState state) => new(id, state);

        private Customer(CustomerId id, CustomerState state) : base(id, state) { }

        public void Handle(Commands.CreateCustomer command)
        {
            EnsureDoesNotExist();

            ApplyState(State with
            {
                Email = command.Email,
                CustomerLevel = new RegularLevel()
            });
        }

        public void Handle(Commands.PlaceOrder command)
        {
            EnsureExists();

            var customerLevel = CustomerLevel.Determine(State, Now);
            var order = CreateOrder(command, customerLevel);

            ApplyState(State with
            {
                Orders = State.Orders.Append(order).ToImmutableArray(),
                CustomerLevel = customerLevel
            });
        }

        Order CreateOrder(Commands.PlaceOrder command, CustomerLevel customerLevel)
        {
            var orderId = new OrderId(command.Id.Value);
            var total = command.Total * ((100 - customerLevel.Discount) / 100);

            return new Order(orderId, Now, total); ;
        }

        Instant Now => NodaTime.SystemClock.Instance.GetCurrentInstant();
    }
}
