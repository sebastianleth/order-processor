using System.Collections.Immutable;
using NodaTime;
using NodaTime.Testing;
using Shouldly;
using Xunit;

namespace OrderProcessor.Domain
{
    public class CustomerLevelCalculatorTests
    {
        static readonly IClock Clock = new FakeClock(SystemClock.Instance.GetCurrentInstant());
        readonly ICustomerLevelCalculator _sut = new CustomerLevelCalculator(Clock);
        readonly Instant _now = Clock.GetCurrentInstant();

        [Fact]
        public void GivenNewCustomer_WhenDetermineLevel_ThenRegular()
        {
            _sut.Determine(new CustomerState())
                .CustomerLevel.ShouldBeOfType<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithSingleOrderInLastThirtyDays_WithSumLargerThan300_WhenDetermineLevel_ThenRegular()
        {
            var stateWithOrders = new CustomerState
            {
                CustomerLevel = new RegularLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 400, 0))
            };

            _sut.Determine(stateWithOrders)
                .CustomerLevel.ShouldBeOfType<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithMultipleOrdersInLastThirtyDays_WithSumLargerThan300_WhenDetermineLevel_ThenSilver()
        {
            var stateWithOrders = new CustomerState
            {
                CustomerLevel = new RegularLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 200, 0),
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 200, 0))
            };

            _sut.Determine(stateWithOrders)
                .CustomerLevel.ShouldBeOfType<SilverLevel>();
        }

        [Fact]
        public void GivenSilverCustomer_WithSingleOrderInLastThirtyDays_WithSumLargerThan600_WithCustomerLevelChanged8DaysAgo_WhenDetermineLevel_ThenGold()
        {
            var stateWithOrders = new CustomerState
            {
                CustomerLevelChangeTime = _now.Minus(Duration.FromDays(8)),
                CustomerLevel = new SilverLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 700, 0))
            };

            _sut.Determine(stateWithOrders)
                .CustomerLevel.ShouldBeOfType<GoldLevel>();
        }

        [Fact]
        public void GivenSilverCustomer_With1OrderInLastThirtyDays_WithSumLargerThan600_WithCustomerLevelChanged7DaysAgo_WhenDetermineLevel_ThenSilver()
        {
            var stateWithOrders = new CustomerState
            {
                CustomerLevelChangeTime = _now.Minus(Duration.FromDays(7)),
                CustomerLevel = new SilverLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 700, 0))
            };

            _sut.Determine(stateWithOrders)
                .CustomerLevel.ShouldBeOfType<SilverLevel>();
        }
    }
}
