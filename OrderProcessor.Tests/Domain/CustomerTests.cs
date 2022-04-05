using System;
using System.Collections.Immutable;
using NodaTime;
using NodaTime.Testing;
using Shouldly;
using Xunit;

namespace OrderProcessor.Domain
{
    public class CustomerTests
    {
        const string CustomerEmail = "email@gmail.com";
        static readonly IClock Clock = new FakeClock(SystemClock.Instance.GetCurrentInstant());
        static readonly Instant _now = Clock.GetCurrentInstant();
        readonly Customer _sut = new(new CustomerId(Guid.NewGuid()), new CustomerState(), Clock, Serilog.Log.Logger);

        [Fact]
        public void GivenNewCustomer_WhenCreate_ThenStateAsExpected()
        {
            _sut.Create(new Commands.CreateCustomer(MessageId.New, CustomerEmail));

            _sut.State
                .ShouldBe(new CustomerState
                {
                    Version = 0,
                    Created = Clock.GetCurrentInstant(),
                    Email = CustomerEmail,
                    Level = Levels.Regular
                });
        }

        [Fact]
        public void GivenNewCustomer_WhenCreateAndPlaceSingleOrder_WithSumMoreThan300_ThenStateAsExpected_AndRegularLevel()
        {
            var orderMessageId = MessageId.New;

            _sut.Create(new Commands.CreateCustomer(MessageId.New, CustomerEmail));
            _sut.PlaceOrder(new Commands.PlaceOrder(orderMessageId, 400, CustomerEmail));

            _sut.State
                .ShouldBe(new CustomerState
                {
                    Orders = ImmutableList
                        .Create(new Order(new OrderId(orderMessageId.Value), _now, Total: 400, DiscountGiven: 0))
                        .WithValueSemantics(),
                    Version = 1,
                    Created = Clock.GetCurrentInstant(),
                    Email = "email@gmail.com",
                    Level = Levels.Regular
                });
        }

        [Fact]
        public void GivenNewCustomer_WhenCreateAndPlaceMultipleOrders_WithSumMoreThan300_ThenStateAsExpected_AndLeveledUpToSilver()
        {
            var orderMessageId = MessageId.New;

            _sut.Create(new Commands.CreateCustomer(MessageId.New, CustomerEmail));
            _sut.PlaceOrder(new Commands.PlaceOrder(orderMessageId, 400, CustomerEmail));
            _sut.PlaceOrder(new Commands.PlaceOrder(orderMessageId, 400, CustomerEmail));

            _sut.State
                .ShouldBe(new CustomerState
                {
                    Orders = ImmutableList
                        .Create(
                            new Order(new OrderId(orderMessageId.Value), _now, Total: 400, DiscountGiven: 0),
                            new Order(new OrderId(orderMessageId.Value), _now, Total: 360, DiscountGiven: 40))
                        .WithValueSemantics(),
                    Version = 2,
                    Created = Clock.GetCurrentInstant(),
                    Email = "email@gmail.com",
                    Level = Levels.Silver,
                    LastLevelUp = _now
                });
        }
    }
}
