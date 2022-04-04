using NodaTime;

namespace OrderProcessor.Domain;

record SilverLevel : ILevel
{
    public string Name => "Silver";
    public decimal DiscountPercentage => 10;
    static decimal OrderSumForLevelUp => 600;

    public LevelResult DetermineLevelUp(CustomerState state, Order placedOrder, Instant now)
    {
        var orderTotals = OrderTotals.Get(state, placedOrder.Total, now);
        var enoughTotalSum = orderTotals.OrderSumLastThirtyDays > OrderSumForLevelUp;
        var lastLevelUpMoreThanAWeekAgo = state.LastLevelUp < now.Minus(Duration.FromDays(7));

        if (enoughTotalSum && lastLevelUpMoreThanAWeekAgo)
        {
            return new LevelResult(new GoldLevel(), LevelUp: true);
        }

        return new LevelResult(this, LevelUp: false);
    }
}