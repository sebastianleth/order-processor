using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Persistence;

namespace OrderProcessor.Domain
{
    public class Customer : Aggregate<CustomerState>
    {
        readonly IClock _clock;

        public Customer(CustomerId id, CustomerState state, IClock clock) : base(id, state)
        {
            _clock = clock;
        }

        public Customer Create(Commands.CreateCustomer command)
        {
            EnsureDoesNotExist();

            ApplyState(State with
            {
                Created = _clock.GetCurrentInstant(),
                Email = command.Email,
                Level = new RegularLevel()
            });

            return this;
        }

        public Order PlaceOrder(Commands.PlaceOrder command)
        {
            EnsureExists();

            var now = _clock.GetCurrentInstant();
            var order = Order.Create(new OrderId(command.Id.Value), command.Total, now);
            var levelResult = State.Level.DetermineLevelUp(State, order, now);

            order = order.ApplyDiscount(levelResult.NextLevel);

            ApplyState(State with
            {
                Orders = State.Orders
                    .Append(order)
                    .ToImmutableList()
                    .WithValueSemantics(),

                Level = levelResult.NextLevel,
                LastLevelUp = levelResult.LevelUp ? _clock.GetCurrentInstant() : State.LastLevelUp
            });

            return order;
        }
    }
}
