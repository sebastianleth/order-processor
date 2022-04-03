namespace OrderProcessor.Ordering;

public record CustomerId(Guid Value) : Aggregates.AggregateId(Value);
