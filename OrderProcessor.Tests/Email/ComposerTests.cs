using System;
using NodaTime;
using OrderProcessor.Commands;
using OrderProcessor.Domain;
using Shouldly;
using Xunit;

namespace OrderProcessor.Email
{
    public class ComposerTests
    {
        readonly IComposer _sut = new Composer();

        [Fact]
        public void GivenParameters_WhenCompose_ThenVeryNice()
        {
            var now = SystemClock.Instance.GetCurrentInstant();
            var yesterday = now.Minus(Duration.FromDays(1));
            var level = new SilverLevel();
            var parameters = new Parameters(
                "email@gmail.com",
                Order.Create(OrderId.New, total: 100, now).ApplyDiscount(level),
                OrderCount: 1,
                OrdersSum: 90,
                Level: level,
                EarliestOrderTime: yesterday,
                LastLevelUpTime: yesterday);


            var expected = $@"
            To:     email@gmail.com

            Your order has been placed.

            Total:      DKK 90,0
            Discount:   DKK 10,0

            You have placed 1 orders for a total of DKK 90 since {yesterday}.

            Your customer level is Silver since {yesterday}, which allows for a 10% discount!
        ";

            var actual = _sut.Do(parameters);

            actual.Body
                .ShouldBe(expected);
        }
    }
}
