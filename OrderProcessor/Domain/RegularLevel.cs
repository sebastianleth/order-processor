using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain;

record RegularLevel : ILevel
{
    public string Name => "Regular";
    public decimal DiscountPercentage => 0;
    static decimal OrderSumForLevelUp => 300;
    static decimal OrderCountForLevelUp => 2;

    public LevelResult DetermineLevelUp(CustomerState state, Order placedOrder, Instant now)
    {
        var orderTotals = OrderTotals.Get(state, placedOrder.Total, now);
        var enoughTotalSum = orderTotals.OrderSumLastThirtyDays > OrderSumForLevelUp;
        var enoughOrders = orderTotals.OrderCountLastThirtyDays >= OrderCountForLevelUp;

        if (enoughTotalSum && enoughOrders)
        {
            return new LevelResult(new SilverLevel(), LevelUp: true);
        }

        return new LevelResult(this, LevelUp: false);
    }
}