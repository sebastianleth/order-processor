namespace OrderProcessor.Ordering
{
    public record PlaceOrderCommand(
        Messaging.MessageId Id,
        int Order
    ) : Messaging.Message(Id);
}