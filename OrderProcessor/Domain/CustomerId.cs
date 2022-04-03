namespace OrderProcessor.Domain;

public record CustomerId(Guid Value) : Aggregates.AggregateId(Value);
