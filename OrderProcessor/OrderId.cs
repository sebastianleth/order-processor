namespace OrderProcessor;

public record OrderId(Guid Value) : EntityId(Value)
{
    public static OrderId New => new(Guid.NewGuid());
}
