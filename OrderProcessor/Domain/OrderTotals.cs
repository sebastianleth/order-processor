using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain;

public record OrderTotals(decimal OrderSumLastThirtyDays, decimal OrderCountLastThirtyDays)
{
    public static OrderTotals Build(CustomerState state, Instant now)
    {
        var thirtyDaysAgo = now.Minus(Duration.FromDays(30));
        var ordersLastThirtyDays = state.Orders.Where(order => order.Time > thirtyDaysAgo).ToImmutableHashSet();
        var orderSum = ordersLastThirtyDays.Sum(order => order.Total);
        var orderCount = ordersLastThirtyDays.Count;

        return new OrderTotals(orderSum, orderCount);
    }
}