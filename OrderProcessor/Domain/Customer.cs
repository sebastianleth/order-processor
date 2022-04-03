using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Persistence;

namespace OrderProcessor.Domain
{
    public class Customer : Aggregate<CustomerState>
    {
        public static Customer Create(CustomerId id, CustomerState state) => new(id, state);

        private Customer(CustomerId id, CustomerState state) : base(id, state) { }

        public Customer Handle(Commands.CreateCustomer command)
        {
            EnsureDoesNotExist();

            ApplyState(State with
            {
                CreatedTime = command.Time,
                Email = command.Email,
                CustomerLevel = new RegularLevel()
            });

            return this;
        }

        public Order Handle(Commands.PlaceOrder command)
        {
            EnsureExists();

            var customerLevel = CustomerLevelCalculator.Determine(State, Now);
            var order = CreateOrder(command, customerLevel);

            ApplyState(State with
            {
                Orders = State.Orders.Append(order).ToImmutableArray(),
                CustomerLevel = customerLevel,
                CustomerLevelChangeTime = CustomerLevelChanged(customerLevel) ? Now : State.CustomerLevelChangeTime
            });

            return order;
        }

        Order CreateOrder(Commands.PlaceOrder command, ICustomerLevel customerLevel)
        {
            var orderId = new OrderId(command.Id.Value);
            var total = command.Total * ((100 - customerLevel.Discount) / 100);

            return new Order(orderId, Now, total);
        }

        bool CustomerLevelChanged(ICustomerLevel customerLevel) => customerLevel.GetType() == State.CustomerLevel.GetType();

        static Instant Now => SystemClock.Instance.GetCurrentInstant();
    }
}
