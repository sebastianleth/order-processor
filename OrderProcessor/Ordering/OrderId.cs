namespace OrderProcessor.Ordering;

public record OrderId(Guid Value) : EntityId(Value);
