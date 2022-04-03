using System.Collections.Immutable;

namespace OrderProcessor.Domain;

public record CustomerState : Aggregates.AggregateState
{
    public CustomerId Id { get; init; }
    public string Email { get; init; }
    public CustomerLevel CustomerLevel { get; init; }
    public ImmutableArray<Order> Orders { get; init; } = ImmutableArray<Order>.Empty;
}