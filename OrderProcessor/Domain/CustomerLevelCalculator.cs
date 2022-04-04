using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain;

public class CustomerLevelCalculator : ICustomerLevelCalculator
{
    private readonly IClock _clock;

    public CustomerLevelCalculator(IClock clock)
    {
        _clock = clock;
    }

    public CustomerLevelResult Determine(CustomerState customerState)
    {
        if (customerState.CustomerLevel is GoldLevel)
        {
            return new CustomerLevelResult(customerState.CustomerLevel, LevelBumped: false);
        }

        var now = _clock.GetCurrentInstant();
        var thirtyDaysAgo = now.Minus(Duration.FromDays(30));
        var ordersLastThirtyDays = customerState.Orders.Where(order => order.Time > thirtyDaysAgo).ToImmutableArray();
        var orderSum = ordersLastThirtyDays.Sum(order => order.Total);

        if (customerState.CustomerLevel is SilverLevel)
        {
            if (orderSum > 600 && customerState.CustomerLevelChangeTime < now.Minus(Duration.FromDays(7)))
            {
                return new CustomerLevelResult(new GoldLevel(), LevelBumped: true);
            }

            return new CustomerLevelResult(customerState.CustomerLevel, LevelBumped: false);
        }

        if (customerState.CustomerLevel is RegularLevel)
        {
            var orderCount = ordersLastThirtyDays.Length;

            if (orderCount >= 2 && orderSum > 300)
            {
                return new CustomerLevelResult(new SilverLevel(), LevelBumped: true);
            }
        }

        return new CustomerLevelResult(customerState.CustomerLevel, LevelBumped: false);
    }
}