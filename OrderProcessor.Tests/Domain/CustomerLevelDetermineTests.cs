using System.Collections.Immutable;
using NodaTime;
using Shouldly;
using Xunit;

namespace OrderProcessor.Domain
{
    public class CustomerLevelDetermineTests
    {
        [Fact]
        public void GivenNewCustomer_WhenDetermineLevel_ThenRegular()
        {
            CustomerLevel.Determine(new CustomerState(), SystemClock.Instance.GetCurrentInstant())
                .ShouldBeAssignableTo<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomerWith1OrderInLastThirtyDaysWithSumLargerThan300_WhenDetermineLevel_ThenRegular()
        {
            var now = SystemClock.Instance.GetCurrentInstant();
            var customerWithOrders = new CustomerState
            {
                CustomerLevel = new RegularLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, now.Minus(Duration.FromDays(1)), 400))
            };

            CustomerLevel.Determine(customerWithOrders, now)
                .ShouldBeAssignableTo<RegularLevel>();
        }

        [Fact]
        public void GivenRegularCustomerWith2OrdersInLastThirtyDaysWithSumLargerThan300_WhenDetermineLevel_ThenSilver()
        {
            var now = SystemClock.Instance.GetCurrentInstant();
            var customerWithOrders = new CustomerState
            {
                CustomerLevel = new RegularLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, now.Minus(Duration.FromDays(1)), 200),
                    new Order(OrderId.New, now.Minus(Duration.FromDays(1)), 200))
            };

            CustomerLevel.Determine(customerWithOrders, now)
                .ShouldBeAssignableTo<SilverLevel>();
        }

        [Fact]
        public void GivenSilverCustomerWith1OrderInLastThirtyDaysWithSumLargerThan600_WhenDetermineLevel_ThenSilver()
        {
            var now = SystemClock.Instance.GetCurrentInstant();
            var customerWithOrders = new CustomerState
            {
                CustomerLevel = new SilverLevel(),
                Orders = ImmutableArray.Create(
                    new Order(OrderId.New, now.Minus(Duration.FromDays(1)), 700))
            };

            CustomerLevel.Determine(customerWithOrders, now)
                .ShouldBeAssignableTo<GoldLevel>();
        }
    }
}
