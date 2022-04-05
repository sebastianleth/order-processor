using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Persistence;
using Serilog;

namespace OrderProcessor.Domain
{
    public class Customer : Aggregate<CustomerState>
    {
        readonly IClock _clock;

        public Customer(CustomerId id, CustomerState state, IClock clock, ILogger logger) : base(id, state, logger)
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
                Level = Levels.Regular
            });

            return this;
        }

        public Order PlaceOrder(Commands.PlaceOrder command)
        {
            EnsureExists();

            var now = _clock.GetCurrentInstant();
            var order = Order.Create(new OrderId(command.Id.Value), command.Total, now);
            var levelResult = State.Level.DetermineLevelUp(State, order, now);

            order = order.ApplyDiscount(levelResult.Level);

            ApplyState(State with
            {
                Orders = State.Orders
                    .Append(order)
                    .ToImmutableList()
                    .WithValueSemantics(),

                Level = levelResult.Level,
                LastLevelUp = levelResult.LevelUp ? _clock.GetCurrentInstant() : State.LastLevelUp
            });

            return order;
        }
    }
}
