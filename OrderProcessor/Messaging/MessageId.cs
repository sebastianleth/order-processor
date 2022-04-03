namespace OrderProcessor.Messaging;

public record MessageId(Guid Value) : EntityId(Value);