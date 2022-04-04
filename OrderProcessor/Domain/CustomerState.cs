using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Persistence;

namespace OrderProcessor.Domain;

public record CustomerState : AggregateState
{
    public string? Email { get; init; }
    public Instant CreatedTime { get; init; }
    public ICustomerLevel CustomerLevel { get; init; } = new RegularLevel();
    public Instant CustomerLevelChangeTime { get; init; }
    public ImmutableArray<Order> Orders { get; init; } = ImmutableArray<Order>.Empty;
}