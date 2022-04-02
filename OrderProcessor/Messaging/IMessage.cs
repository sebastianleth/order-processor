namespace OrderProcessor.Messaging
{
    public interface IMessage
    {
        MessageId Id { get; }
    }
}
