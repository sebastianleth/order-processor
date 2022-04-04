using NodaTime;

namespace OrderProcessor.Domain;

class GoldLevel : ILevel
{
    public string Name => "Gold";
    public decimal DiscountPercentage => 15;

    public LevelResult DetermineLevelUp(CustomerState state, Instant now) => new(this, LevelUp: false);
}