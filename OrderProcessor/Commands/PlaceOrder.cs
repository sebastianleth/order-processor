using System.Collections.Immutable;
using NodaTime;
using OrderProcessor.Domain;

namespace OrderProcessor.Commands
{
    public record PlaceOrder(
        Messaging.MessageId Id,
        Instant Time,
        int Total,
    ) : Messaging.Message(Id);
}