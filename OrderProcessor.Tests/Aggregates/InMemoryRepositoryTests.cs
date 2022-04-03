using System;
using System.Threading.Tasks;
using NodaTime;
using OrderProcessor.Commands;
using OrderProcessor.Domain;
using OrderProcessor.Messaging;
using OrderProcessor.Persistence;
using Shouldly;
using Xunit;

namespace OrderProcessor.Aggregates
{
    public class InMemoryRepositoryTests
    {
        readonly IAggregateRepository _sut = new InMemoryRepository();

        [Fact]
        public async Task GivenNotExistingAggregate_WhenLoad_ThenFail()
        {
            var missingAggregateId = CustomerId.FromEmail("sebastian@koderi.dk");

            var exception = await Should.ThrowAsync<DomainException>(async() => await _sut.Load<CustomerId, Customer, CustomerState>(missingAggregateId, Customer.Create));

            exception.Message
                .ShouldBe($"Customer {missingAggregateId} does not exist");
        }

        [Fact]
        public async Task GivenExistingAggregate_WhenLoad_ThenAggregateLoaded()
        {
            var aggregateId = CustomerId.FromEmail("sebastian@koderi.dk");
            var expected = Customer.Create(aggregateId, new CustomerState());
            await _sut.Save<CustomerId, Customer, CustomerState>(expected);

            var actual = await _sut.Load<CustomerId, Customer, CustomerState>(aggregateId, Customer.Create);

            actual.Id
                .ShouldBe(expected.Id);

            actual.State
                .ShouldBe(expected.State);
        }

        [Fact]
        public async Task GivenExistingChangedAggregate_WhenSaveUnchangedSecondInstanceOfSameAggregate_ThenFailByOptimisticConcurrency()
        {
            var aggregateId = CustomerId.FromEmail("sebastian@koderi.dk");
            var time = Instant.MaxValue;
            var aggregate = Customer.Create(aggregateId, new CustomerState());
            await _sut.Save<CustomerId, Customer, CustomerState>(aggregate);

            var firstInstance = await _sut.Load<CustomerId, Customer, CustomerState>(aggregateId, Customer.Create);
            var secondInstance = await _sut.Load<CustomerId, Customer, CustomerState>(aggregateId, Customer.Create);

            firstInstance.Handle(new CreateCustomer(MessageId.New, time, "first@gmail.com"));
            await _sut.Save<CustomerId, Customer, CustomerState>(firstInstance);

            secondInstance.Handle(new CreateCustomer(MessageId.New, time, "first@gmail.com"));
            var exception = await Should.ThrowAsync<DomainException>(async () => await _sut.Save<CustomerId, Customer, CustomerState>(secondInstance));

            exception.Message
                .ShouldBe($"Customer {secondInstance.Id} was changed by another actor");
        }
    }
}
