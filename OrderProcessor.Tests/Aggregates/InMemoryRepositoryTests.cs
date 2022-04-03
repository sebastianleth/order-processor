using System;
using System.Threading.Tasks;
using OrderProcessor.Messaging;
using OrderProcessor.Ordering;
using Shouldly;
using Xunit;

namespace OrderProcessor.Aggregates
{
    public class InMemoryRepositoryTests
    {
        readonly IAggregateRepository _sut = new InMemoryRepository();

        [Fact]
        public async Task GivenNotExistingId_WhenLoad_ThenFail()
        {
            var missingAggregateId = new AggregateId(Guid.NewGuid());

            var exception = await Should.ThrowAsync<DomainException>(async() => await _sut.Load<Ordering.Customer, Ordering.CustomerState>(missingAggregateId, Ordering.Customer.Create));

            exception.Message
                .ShouldBe($"Customer {missingAggregateId} does not exist");
        }

        [Fact]
        public async Task GivenExistingId_WhenLoad_ThenAggregateLoaded()
        {
            var aggregateId = new AggregateId(Guid.NewGuid());
            var expected = Ordering.Customer.Create(aggregateId, new Ordering.CustomerState());
            await _sut.Save<Ordering.Customer, Ordering.CustomerState>(expected);

            var actual = await _sut.Load<Ordering.Customer, Ordering.CustomerState>(aggregateId, Ordering.Customer.Create);

            actual.Id
                .ShouldBe(expected.Id);

            actual.State
                .ShouldBe(expected.State);
        }

        [Fact]
        public async Task GivenExistingAndChangedAggregated_WhenSave_ThenFailByOptimisticConcurrency()
        {
            var aggregateId = new AggregateId(Guid.NewGuid());
            var expected = Ordering.Customer.Create(aggregateId, new Ordering.CustomerState());
            await _sut.Save<Ordering.Customer, Ordering.CustomerState>(expected);

            var firstAggregateInstance = await _sut.Load<Ordering.Customer, Ordering.CustomerState>(aggregateId, Ordering.Customer.Create);
            var secondAggregateInstance = await _sut.Load<Ordering.Customer, Ordering.CustomerState>(aggregateId, Ordering.Customer.Create);

            firstAggregateInstance.Handle(new CreateCustomerCommand(new MessageId(Guid.NewGuid()), "first@gmail.com"));
            firstAggregateInstance.Handle(new PlaceOrderCommand(new MessageId(Guid.NewGuid()), 10));
            await _sut.Save<Ordering.Customer, Ordering.CustomerState>(firstAggregateInstance);

            secondAggregateInstance.Handle(new CreateCustomerCommand(new MessageId(Guid.NewGuid()), "second@gmail.com"));
            var exception = await Should.ThrowAsync<DomainException>(async () => await _sut.Save<Ordering.Customer, Ordering.CustomerState>(secondAggregateInstance));

            exception.Message
                .ShouldBe($"Customer {secondAggregateInstance.Id} was changed by another actor");
        }
    }
}
