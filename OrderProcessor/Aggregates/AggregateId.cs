namespace OrderProcessor.Aggregates;

public record AggregateId(Guid Value) : EntityId(Value);