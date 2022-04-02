using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace OrderProcessor.Messaging
{
    public class ClientTests
    {
        readonly IClient _sut = new InMemQueueClient();
        readonly ImmutableArray<EnqueuedMessage> _messages;

        public ClientTests()
        {
            _messages = EnqueueMessages().Result;
        }

        [Fact]
        public async Task GivenEnqueuedMessages_WhenDequeuingAll_ThenOrderingAndIdsAsExpected()
        {
            var actuals = new List<Message>();

            while(await _sut.TryDequeue(out EnqueuedMessage? outMessage))
            {
                actuals.Add(outMessage!);
            }

            actuals
                .ShouldBe(_messages);
        }

        [Fact]
        public async Task GivenEnqueuedMessages_WhenDequeueTypeNotEnqueued_ThenTryFail()
        {
            (await _sut.TryDequeue(out NotEnqueuedMessage? outMessage))
                .ShouldBeFalse();
        }

        [Fact]
        public async Task GivenEnqueuedMessages_WhenDequeueTypeNotEnqueued_ThenMessageNull()
        {
            await _sut.TryDequeue(out NotEnqueuedMessage? outMessage);

            outMessage
                .ShouldBeNull();
        }

        async Task<ImmutableArray<EnqueuedMessage>> EnqueueMessages()
        {
            var orderedMessages = Enumerable.Range(0, 3)
                .Select(index => new EnqueuedMessage(new MessageId(Guid.NewGuid()), index))
                .ToImmutableArray();

            foreach (var message in orderedMessages)
                await _sut.Enqueue(message);

            return orderedMessages;
        }
    }
}
