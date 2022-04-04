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

        public Customer Handle(Commands.CreateCustomer command)
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

        public Order Handle(Commands.PlaceOrder command)
        {
            EnsureExists();

            var now = _clock.GetCurrentInstant();
            var levelResult = State.Level.DetermineLevelUp(State, now);
            var order = Order.Create(command.Id.ToOrderId, command.Total, levelResult.NextLevel, now);

            ApplyState(State with
            {
                Orders = State.Orders
                    .Append(order)
                    .ToImmutableArray(),

                Level = levelResult.NextLevel,
                LastLevelUp = levelResult.LevelUp ? _clock.GetCurrentInstant() : State.LastLevelUp
            });

            return order;
        }
    }
}
