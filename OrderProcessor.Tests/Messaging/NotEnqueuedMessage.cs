namespace OrderProcessor.Messaging
{
    public record NotEnqueuedMessage(MessageId Id) : Message(Id);
}
