namespace OrderProcessor.Messaging
{
    class InMemQueueClient : IClient
    {
        readonly System.Collections.Concurrent.ConcurrentQueue<Message> _queue = new();

        public Task<bool> TryDequeue<T>(out T? message) where T : Message
        {
            var found = _queue.TryPeek(out var dequeuedMessage);

            if (found && dequeuedMessage is T)
            {
                message = (T) dequeuedMessage;
                _queue.TryDequeue(out dequeuedMessage);

                return Task.FromResult(true);
            }

            message = null;
            return Task.FromResult(false);
        }

        public Task Enqueue<T>(T message) where T : Message
        {
            _queue.Enqueue(message);

            return Task.CompletedTask;
        }
    }
}
