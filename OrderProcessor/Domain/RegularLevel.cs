using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain;

class RegularLevel : ILevel
{
    public string Name => "Regular";
    public decimal DiscountPercentage => 0;
    decimal OrderSumForLevelUp => 300;
    decimal OrderCountForLevelUp => 2;

    public LevelResult DetermineLevelUp(CustomerState state, Instant now)
    {
        var orderTotals = OrderTotals.Build(state, now);
        var enoughTotalSum = orderTotals.OrderSumLastThirtyDays > OrderSumForLevelUp;
        var enoughOrders = orderTotals.OrderCountLastThirtyDays >= OrderCountForLevelUp;

        if (enoughTotalSum && enoughOrders)
        {
            return new LevelResult(new SilverLevel(), LevelUp: true);
        }

        return new LevelResult(this, LevelUp: false);
    }
}