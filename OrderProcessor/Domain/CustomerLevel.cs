using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain;

public abstract class CustomerLevel
{
    public abstract string Name { get; }
    public abstract decimal Discount { get; }

    public static CustomerLevel Determine(CustomerState state, Instant now)
    {
        if (state.CustomerLevel is GoldLevel)
            return state.CustomerLevel;

        var thirtyDaysAgo = now.Minus(Duration.FromDays(30));
        var ordersLastThirtyDays = state.Orders.Where(order => order.Time > thirtyDaysAgo).ToImmutableArray();
        var orderSum = ordersLastThirtyDays.Sum(order => order.Total);

        if (state.CustomerLevel is SilverLevel)
        {
            if (orderSum > 600 && state.CustomerLevelChangeTime < now.Minus(Duration.FromDays(7)))
            {
                return new GoldLevel();
            }

            return state.CustomerLevel;
        }

        if (state.CustomerLevel is RegularLevel)
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