using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Persistence;

namespace OrderProcessor.Domain
{
    public class Customer : Aggregate<CustomerState>
    {
        public static Customer New(CustomerId id) => new(id, new CustomerState());

        public static Customer Initialize(CustomerId id, CustomerState state) => new(id, state);

        private Customer(CustomerId id, CustomerState state) : base(id, state) { }

        public Customer Handle(Commands.CreateCustomer command)
        {
            EnsureDoesNotExist();

            ApplyState(State with
            {
                CreatedTime = Now,
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

        static Order CreateOrder(Commands.PlaceOrder command, ICustomerLevel customerLevel)
        {
            var orderId = new OrderId(command.Id.Value);

            var discountGiven = (customerLevel.DiscountPercentage / 100) * command.Total;
            var total = command.Total - discountGiven;

            return new Order(orderId, Now, total, discountGiven);
        }

        bool CustomerLevelChanged(ICustomerLevel customerLevel) => customerLevel.GetType() != State.CustomerLevel.GetType();

        static Instant Now => SystemClock.Instance.GetCurrentInstant();
    }
}
