using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace OrderProcessor.Messaging
{
    public class InMemoryQueueClientTests
    {
        readonly IClient _sut = new InMemoryQueueClient();
        readonly ImmutableArray<EnqueuedMessage> _expectedMessages;

        public InMemoryQueueClientTests()
        {
            _expectedMessages = EnqueueMessages().Result;
        }

        [Fact]
        public async Task GivenEnqueuedMessages_WhenDequeuingAll_ThenOrderingAndIdsAsExpected()
        {
            var actualMessages = new List<Message>();

            while(await _sut.TryDequeue(out EnqueuedMessage? outMessage))
            {
                actualMessages.Add(outMessage!);
            }

            actualMessages
                .ShouldBe(_expectedMessages);
        }

        [Fact]
        public async Task GivenEnqueuedMessages_WhenDequeueTypeNotEnqueued_ThenTryFail()
        {
            (await _sut.TryDequeue(out NotEnqueuedMessage? _))
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
        public record EnqueuedMessage(MessageId Id, int Order) : Message(Id);

        public record NotEnqueuedMessage(MessageId Id) : Message(Id);
    }
}
