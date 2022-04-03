using System.Collections.Immutable;
using NodaTime;
using Shouldly;
using Xunit;

namespace OrderProcessor.Domain
{
    public class CustomerLevelDetermineTests
    {
        readonly Instant _now = SystemClock.Instance.GetCurrentInstant();

        [Fact]
        public void GivenNewCustomer_WhenDetermineLevel_ThenRegular()
        {
            CustomerLevel.Determine(new CustomerState(), _now)
                .ShouldBeOfType<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithSingleOrderInLastThirtyDays_WithSumLargerThan300_WhenDetermineLevel_ThenRegular()
        {
            var customerWithOrders = new CustomerState
            {
                CustomerLevel = new RegularLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 400))
            };

            CustomerLevel.Determine(customerWithOrders, _now)
                .ShouldBeOfType<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomer_WithMultipleOrdersInLastThirtyDays_WithSumLargerThan300_WhenDetermineLevel_ThenSilver()
        {
            var customerWithOrders = new CustomerState
            {
                CustomerLevel = new RegularLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 200),
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 200))
            };

            CustomerLevel.Determine(customerWithOrders, _now)
                .ShouldBeOfType<SilverLevel>();
        }

        [Fact]
        public void GivenSilverCustomer_WithSingleOrderInLastThirtyDays_WithSumLargerThan600_WithCustomerLevelChanged8DaysAgo_WhenDetermineLevel_ThenGold()
        {
            var customerWithOrders = new CustomerState
            {
                CustomerLevelChangeTime = _now.Minus(Duration.FromDays(8)),
                CustomerLevel = new SilverLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 700))
            };

            CustomerLevel.Determine(customerWithOrders, _now)
                .ShouldBeOfType<GoldLevel>();
        }

        [Fact]
        public void GivenSilverCustomer_With1OrderInLastThirtyDays_WithSumLargerThan600_WithCustomerLevelChanged7DaysAgo_WhenDetermineLevel_ThenSilver()
        {
            var customerWithOrders = new CustomerState
            {
                CustomerLevelChangeTime = _now.Minus(Duration.FromDays(7)),
                CustomerLevel = new SilverLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, _now.Minus(Duration.FromDays(1)), 700))
            };

            CustomerLevel.Determine(customerWithOrders, _now)
                .ShouldBeOfType<SilverLevel>();
        }
    }
}
