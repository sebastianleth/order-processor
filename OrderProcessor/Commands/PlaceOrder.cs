using System.Collections.Immutable;
using NodaTime;

namespace OrderProcessor.Commands
{
    public record PlaceOrder(
        Messaging.MessageId Id,
        Instant Time,
        decimal Total,
        string CustomerEmail
    ) : Messaging.Message(Id), ICommand;
}