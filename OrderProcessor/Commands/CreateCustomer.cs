using NodaTime;

namespace OrderProcessor.Commands
{
    public record CreateCustomer(
        Messaging.MessageId Id,
        Instant Time,
        string Email
    ) : Messaging.Message(Id);
}