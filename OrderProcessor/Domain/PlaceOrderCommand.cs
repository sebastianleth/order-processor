namespace OrderProcessor.Domain
{
    public record PlaceOrderCommand(
        Messaging.MessageId Id,
        int Order
    ) : Messaging.Message(Id);
}