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
        public void GivenLevels_WhenCompare_ThenByValueAsExpected()
        {
            new RegularLevel().ShouldBe(new RegularLevel());
            new SilverLevel().ShouldBe(new SilverLevel());
            new GoldLevel().ShouldBe(new GoldLevel());
        }

        [Fact]
        public void GivenLevelsFactory_WhenCompare_ThenAsExpected()
        {
            Levels.Regular.ShouldBe(new RegularLevel());
            Levels.Silver.ShouldBe(new SilverLevel());
            Levels.Gold.ShouldBe(new GoldLevel());
        }

        [Fact]
        public void GivenRegularCustomer_WhenGetDiscount_ThenZero()
        {
            Levels.Regular.DiscountPercentage
                .ShouldBe(0);
        }

        [Fact]
        public void GivenSilverCustomer_WhenGetDiscount_ThenTen()
        {
            Levels.Silver.DiscountPercentage
                .ShouldBe(10);
        }

        [Fact]
        public void GivenGoldCustomer_WhenGetDiscount_ThenFifteen()
        {
            Levels.Gold.DiscountPercentage
                .ShouldBe(15);
        }

        [Fact]
        public void GivenNewCustomer_WithZeroOrders_WhenDetermineLevel_WithTotalOrderSumOfZero_ThenRegular()
        {
            var placedOrder = Order.Create(OrderId.New, 0, _now);
            Levels.Regular.DetermineLevelUp(new CustomerState(), placedOrder, _now)
                .Level.ShouldBeOfType<RegularLevel>();
        }

        [Fact]
        public void GivenNewCustomer_WithZeroOrders_WhenDetermineLevel_WithTotalOrderSumLargerThan300_ThenRegular()
        {
            var placedOrder = Order.Create(OrderId.New, 400, _now);
            Levels.Regular.DetermineLevelUp(new CustomerState(), placedOrder, _now)
                .Level.ShouldBeOfType<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithSingleOrderInLastThirtyDays_WithTotalOrderSumLargerThan300_WhenDetermineLevel_ThenSilver_1()
        {
            var stateWithOrders = new CustomerState
            {
                Level = Levels.Regular,
                Orders = ImmutableList
                    .Create(new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 400, 0))
                    .WithValueSemantics()
            };

            var placedOrder = Order.Create(OrderId.New, 0, _now);
            Levels.Regular.DetermineLevelUp(stateWithOrders, placedOrder, _now)
                .Level.ShouldBeOfType<SilverLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithSingleOrderInLastThirtyDays_WithTotalOrderSumLargerThan300_WhenDetermineLevel_ThenSilver_2()
        {
            var stateWithOrders = new CustomerState
            {
                Level = Levels.Regular,
                Orders = ImmutableList
                    .Create(new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 100, 0))
                    .WithValueSemantics()
            };

            var placedOrder = Order.Create(OrderId.New, 300, _now);
            Levels.Regular.DetermineLevelUp(stateWithOrders, placedOrder, _now)
                .Level.ShouldBeOfType<SilverLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithMultipleOrdersInLastThirtyDays_ithTotalOrderSumLargerThan300_WhenDetermineLevel_ThenSilver()
        {
            var stateWithOrders = new CustomerState
            {
                Level = Levels.Regular,
                Orders = ImmutableList
                    .Create(
                        new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 200, 0),
                        new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 200, 0))
                    .WithValueSemantics()
            };

            var placedOrder = Order.Create(OrderId.New, 0, _now);
            Levels.Regular.DetermineLevelUp(stateWithOrders, placedOrder, _now)
                .Level.ShouldBeOfType<SilverLevel>();
        }

        [Fact]
        public void GivenSilverCustomer_WithSingleOrderInLastThirtyDays_WithSumLargerThan600_WithCustomerLevelChanged8DaysAgo_WhenDetermineLevel_ThenGold()
        {
            var stateWithOrders = new CustomerState
            {
                LastLevelUp = _now.Minus(Duration.FromDays(8)),
                Level = Levels.Silver,
                Orders = ImmutableList
                            .Create(new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 700, 0))
                            .WithValueSemantics()
            };

            var placedOrder = Order.Create(OrderId.New, 0, _now);
            Levels.Silver.DetermineLevelUp(stateWithOrders, placedOrder, _now)
                .Level.ShouldBeOfType<GoldLevel>();
        }

        [Fact]
        public void GivenSilverCustomer_WithSingleOrderInLastThirtyDays_WithSumLargerThan600_WithCustomerLevelChanged7DaysAgo_WhenDetermineLevel_ThenSilver()
        {
            var placedOrder = Order.Create(OrderId.New, 0, _now);
            var stateWithOrders = new CustomerState
            {
                LastLevelUp = _now.Minus(Duration.FromDays(7)),
                Level = Levels.Silver,
                Orders = ImmutableList
                            .Create(new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 700, 0))
                            .WithValueSemantics()
            };

            Levels.Silver.DetermineLevelUp(stateWithOrders, placedOrder, _now)
                .Level.ShouldBeOfType<SilverLevel>();
        }

        [Fact]
        public void GivenNewButGoldCustomer_WithZeroOrders_WhenDetermineLevel_WithTotalOrderSumOfZero_ThenStillGold()
        {
            var placedOrder = Order.Create(OrderId.New, 0, _now);
            var state = new CustomerState { Level = Levels.Gold };

            state.Level.DetermineLevelUp(state, placedOrder, _now)
                .Level.ShouldBeOfType<GoldLevel>();
        }
    }
}
