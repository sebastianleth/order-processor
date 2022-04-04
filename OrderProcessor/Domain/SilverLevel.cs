using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Domain;

class SilverLevel : ILevel
{
    public string Name => "Silver";
    public decimal DiscountPercentage => 10;
    decimal OrderSumForLevelUp => 600;

    public LevelResult DetermineLevelUp(CustomerState state, Instant now)
    {
        var orderTotals = OrderTotals.Build(state, now);
        var enoughTotalSum = orderTotals.OrderSumLastThirtyDays > OrderSumForLevelUp;
        var lastLevelUpMoreThanAWeekAgo = state.LastLevelUp < now.Minus(Duration.FromDays(7));

        if (enoughTotalSum && lastLevelUpMoreThanAWeekAgo)
        {
            return new LevelResult(new GoldLevel(), LevelUp: true);
        }

        return new LevelResult(this, LevelUp: false);
    }
}