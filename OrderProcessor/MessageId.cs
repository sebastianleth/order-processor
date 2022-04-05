using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderProcessor;

public record MessageId(Guid Value) : EntityId(Value)
{
    public static MessageId New => new(Guid.NewGuid());
}