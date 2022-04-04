using System.Text.Json.Serialization;

namespace OrderProcessor;

public record MessageId(Guid Value) : EntityId(Value)
{
    public static MessageId New => new(Guid.NewGuid());

    [JsonIgnore]
    public OrderId ToOrderId => new(Value);
}