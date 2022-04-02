namespace OrderProcessor.Messaging
{
    public record EnqueuedMessage(
        MessageId Id,
        int Order
    ) : Message(Id);
}
