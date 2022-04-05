using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain;

public record OrderTotals(decimal OrderSumLastThirtyDays, decimal OrderCountLastThirtyDays)
{
    public static OrderTotals Get(CustomerState state, decimal placedOrderTotal, Instant now)
    {
        var thirtyDaysAgo = now.Minus(Duration.FromDays(30));
        var ordersLastThirtyDays = state.Orders.Where(order => order.Time > thirtyDaysAgo).ToImmutableHashSet();
        var orderSum = ordersLastThirtyDays.Sum(order => order.Total) + placedOrderTotal;
        var orderCount = ordersLastThirtyDays.Count + 1;

        return new OrderTotals(orderSum, orderCount);
    }
}