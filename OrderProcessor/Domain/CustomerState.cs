using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Persistence;

namespace OrderProcessor.Domain;

public record CustomerState : AggregateState
{
    public string? Email { get; init; }
    public Instant Created { get; init; }
    public ILevel Level { get; init; } = Levels.Regular;
    public Instant LastLevelUp { get; init; }
    public ImmutableListWithValueSemantics<Order> Orders { get; init; } = ImmutableList<Order>.Empty.WithValueSemantics();
}