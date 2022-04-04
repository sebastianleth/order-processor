using System.Collections.Immutable;
using NodaTime;
using NodaTime.Testing;
using Shouldly;
using Xunit;

namespace OrderProcessor.Domain
{
    public class LevelTests
    {
        static readonly IClock Clock = new FakeClock(SystemClock.Instance.GetCurrentInstant());
        readonly Instant _now = Clock.GetCurrentInstant();

        [Fact]
        public void GivenRegularCustomer_WhenGetDiscount_ThenZero()
        {
            new RegularLevel().DiscountPercentage
                .ShouldBe(0);
        }

        [Fact]
        public void GivenSilverCustomer_WhenGetDiscount_ThenTen()
        {
            new SilverLevel().DiscountPercentage
                .ShouldBe(10);
        }

        [Fact]
        public void GivenGoldCustomer_WhenGetDiscount_ThenFifteen()
        {
            new GoldLevel().DiscountPercentage
                .ShouldBe(15);
        }

        [Fact]
        public void GivenNewCustomer_WhenDetermineLevel_ThenRegular()
        {
            new RegularLevel().DetermineLevelUp(new CustomerState(), _now)
                .NextLevel.ShouldBeOfType<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithSingleOrderInLastThirtyDays_WithSumLargerThan300_WhenDetermineLevel_ThenRegular()
        {
            var stateWithOrders = new CustomerState
            {
                Level = new RegularLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 400, 0))
            };

            new RegularLevel().DetermineLevelUp(stateWithOrders, _now)
                .NextLevel.ShouldBeOfType<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithMultipleOrdersInLastThirtyDays_WithSumLargerThan300_WhenDetermineLevel_ThenSilver()
        {
            var stateWithOrders = new CustomerState
            {
                Level = new RegularLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 200, 0),
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 200, 0))
            };

            new RegularLevel().DetermineLevelUp(stateWithOrders, _now)
                .NextLevel.ShouldBeOfType<SilverLevel>();
        }

        [Fact]
        public void GivenSilverCustomer_WithSingleOrderInLastThirtyDays_WithSumLargerThan600_WithCustomerLevelChanged8DaysAgo_WhenDetermineLevel_ThenGold()
        {
            var stateWithOrders = new CustomerState
            {
                LastLevelUp = _now.Minus(Duration.FromDays(8)),
                Level = new SilverLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 700, 0))
            };

            new SilverLevel().DetermineLevelUp(stateWithOrders, _now)
                .NextLevel.ShouldBeOfType<GoldLevel>();
        }

        [Fact]
        public void GivenSilverCustomer_With1OrderInLastThirtyDays_WithSumLargerThan600_WithCustomerLevelChanged7DaysAgo_WhenDetermineLevel_ThenSilver()
        {
            var stateWithOrders = new CustomerState
            {
                LastLevelUp = _now.Minus(Duration.FromDays(7)),
                Level = new SilverLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 700, 0))
            };

            new SilverLevel().DetermineLevelUp(stateWithOrders, _now)
                .NextLevel.ShouldBeOfType<SilverLevel>();
        }
    }
}
