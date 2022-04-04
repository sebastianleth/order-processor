using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Persistence;

namespace OrderProcessor.Domain
{
    public class Customer : Aggregate<CustomerState>
    {
        readonly ICustomerLevelCalculator _customerLevelCalculator;
        readonly IClock _clock;

        public Customer(CustomerId id, CustomerState state, ICustomerLevelCalculator customerLevelCalculator, IClock clock) : base(id, state)
        {
            _customerLevelCalculator = customerLevelCalculator;
            _clock = clock;
        }

        public Customer Handle(Commands.CreateCustomer command)
        {
            EnsureDoesNotExist();

            ApplyState(State with
            {
                CreatedTime = _clock.GetCurrentInstant(),
                Email = command.Email,
                CustomerLevel = new RegularLevel()
            });

            return this;
        }

        public Order Handle(Commands.PlaceOrder command)
        {
            EnsureExists();

            var customerLevelResult = _customerLevelCalculator.Determine(State);
            var order = CreateOrder(command, customerLevelResult.CustomerLevel);

            ApplyState(State with
            {
                Orders = State.Orders.Append(order).ToImmutableArray(),
                CustomerLevel = customerLevelResult.CustomerLevel,
                CustomerLevelChangeTime = customerLevelResult.LevelBumped ? _clock.GetCurrentInstant() : State.CustomerLevelChangeTime
            });

            return order;
        }

        Order CreateOrder(Commands.PlaceOrder command, ICustomerLevel customerLevel)
        {
            var orderId = new OrderId(command.Id.Value);

            var discountGiven = (customerLevel.DiscountPercentage / 100) * command.Total;
            var total = command.Total - discountGiven;

            return new Order(orderId, _clock.GetCurrentInstant(), total, discountGiven);
        }

        bool CustomerLevelChanged(ICustomerLevel customerLevel) => customerLevel.GetType() != State.CustomerLevel.GetType();
    }
}
