using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain;

public static class CustomerLevelCalculator
{
    public static ICustomerLevel Determine(CustomerState customerState, Instant now)
    {
        if (customerState.CustomerLevel is GoldLevel)
        {
            return customerState.CustomerLevel;
        }

        var thirtyDaysAgo = now.Minus(Duration.FromDays(30));
        var ordersLastThirtyDays = customerState.Orders.Where(order => order.Time > thirtyDaysAgo).ToImmutableArray();
        var orderSum = ordersLastThirtyDays.Sum(order => order.Total);

        if (customerState.CustomerLevel is SilverLevel)
        {
            if (orderSum > 600 && customerState.CustomerLevelChangeTime < now.Minus(Duration.FromDays(7)))
            {
                return new GoldLevel();
            }

            return customerState.CustomerLevel;
        }

        if (customerState.CustomerLevel is RegularLevel)
        {
            var orderCount = ordersLastThirtyDays.Length;

            if (orderCount >= 2 && orderSum > 300)
            {
                return new SilverLevel();
            }
        }

        return new RegularLevel();
    }
}