namespace OrderProcessor.Persistence;

public record AggregateId(Guid Value) : EntityId(Value);