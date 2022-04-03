using System.Collections.Immutable;
using NodaTime;
using Shouldly;
using Xunit;

namespace OrderProcessor.Domain
{
    public class CustomerLevelTests
    {
        [Fact]
        public void GivenRegularCustomer_WhenGetDiscount_ThenZero()
        {
            new RegularLevel().Discount
                .ShouldBe(0);
        }

        [Fact]
        public void GivenSilverCustomer_WhenGetDiscount_ThenTen()
        {
            new SilverLevel().Discount
                .ShouldBe(10);
        }

        [Fact]
        public void GivenGoldCustomer_WhenGetDiscount_ThenFifteen()
        {
            new GoldLevel().Discount
                .ShouldBe(15);
        }
    }
}
