namespace OrderProcessor.Domain;

public record OrderId(Guid Value) : EntityId(Value);
