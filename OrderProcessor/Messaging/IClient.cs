namespace OrderProcessor.Messaging
{
    public interface IClient
    {
        Task Enqueue<T>(T message) where T : Message;

        Task<bool> TryDequeue<T>(out T? message) where T : Message;
    }
}
