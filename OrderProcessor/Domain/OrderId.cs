namespace OrderProcessor.Domain;

public record OrderId(Guid Value) : EntityId(Value)
{
    public static OrderId New => new OrderId(Guid.NewGuid());
}
