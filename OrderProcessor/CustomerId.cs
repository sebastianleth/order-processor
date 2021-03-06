using OrderProcessor.Messaging;
using OrderProcessor.Persistence;

namespace OrderProcessor;

public record CustomerId(Guid Value) : AggregateId(Value)
{
    static readonly Guid Namespace = Guid.Parse("1e38de13-46e2-495b-800f-6a62990985b1");

    public static CustomerId FromEmail(string email) => new (DeterministicGuid.Create(Namespace, email));
}
