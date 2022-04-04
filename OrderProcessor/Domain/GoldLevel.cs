using NodaTime;

namespace OrderProcessor.Domain;

record GoldLevel : ILevel
{
    public string Name => "Gold";
    public decimal DiscountPercentage => 15;

    public LevelResult DetermineLevelUp(CustomerState state, Order placedOrder, Instant now) => new(this, LevelUp: false);
}