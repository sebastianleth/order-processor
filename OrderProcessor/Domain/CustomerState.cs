using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Persistence;

namespace OrderProcessor.Domain;

public record CustomerState : AggregateState
{
    public string? Email { get; init; }
    public Instant Created { get; init; }
    public ILevel Level { get; init; } = new RegularLevel();
    public Instant LastLevelUp { get; init; }
    public ImmutableArray<Order> Orders { get; init; } = ImmutableArray<Order>.Empty;
}