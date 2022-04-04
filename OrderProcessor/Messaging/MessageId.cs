namespace OrderProcessor.Messaging;

public record MessageId(Guid Value) : EntityId(Value)
{
    public static MessageId New => new(Guid.NewGuid());

    public OrderId ToOrderId => new(Value);
}