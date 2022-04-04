using System.Threading.Tasks;
using NodaTime;
using OrderProcessor.Commands;
using OrderProcessor.Domain;
using OrderProcessor.Messaging;
using Shouldly;
using Xunit;

namespace OrderProcessor.Persistence
{
    public class InMemoryRepositoryTests
    {
        readonly IAggregateRepository<CustomerId, Customer> _sut = new InMemoryAggregateRepository<CustomerId, Customer, CustomerState>(Factory);

        static Customer Factory(CustomerId id, CustomerState state) => new(id, state, SystemClock.Instance);


        [Fact]
        public async Task GivenNotExistingAggregate_WhenLoad_ThenFail()
        {
            var missingAggregateId = CustomerId.FromEmail("email@gmail.com");

            var exception = await Should.ThrowAsync<DomainException>(async() => await _sut.Load(missingAggregateId));

            exception.Message
                .ShouldBe($"Customer {missingAggregateId} does not exist");
        }

        [Fact]
        public async Task GivenExistingAggregate_WhenLoad_ThenAggregateLoaded()
        {
            var aggregateId = CustomerId.FromEmail("email@gmail.com");
            var expected = await _sut.New(aggregateId);
            await _sut.Insert(expected);

            var actual = await _sut.Load(aggregateId);

            actual.Id
                .ShouldBe(expected.Id);

            actual.State
                .ShouldBe(expected.State);
        }

        [Fact]
        public async Task GivenNotExistingAggregate_WhenInsertNew_ThenOk()
        {
            var newAggregateId = CustomerId.FromEmail("email@gmail.com");
            var customer = await _sut.New(newAggregateId);

            await _sut.Save(customer);
        }

        [Fact]
        public async Task GivenExistingAggregate_WhenInsertNew_ThenFailByAlreadyExisting()
        {
            var aggregateId = CustomerId.FromEmail("email@gmail.com");
            var customer = await _sut.New(aggregateId);

            await _sut.Insert(customer);

            customer = await _sut.New(aggregateId);
            var exception = await Should.ThrowAsync<DomainException>(async () => await _sut.Insert(customer));

            exception.Message
                .ShouldBe($"Customer {customer.Id} already exists, and cannot be created anew");
        }

        [Fact]
        public async Task GivenExistingChangedAggregate_WhenSaveUnchangedSecondInstanceOfSameAggregate_ThenFailByOptimisticConcurrency()
        {
            var aggregateId = CustomerId.FromEmail("email@gmail.com");
            var aggregate = await _sut.New(aggregateId);
            await _sut.Save(aggregate);

            var firstInstance = await _sut.Load(aggregateId);
            var secondInstance = await _sut.Load(aggregateId);

            firstInstance.Handle(new CreateCustomer(MessageId.New, "email@gmail.com"));
            await _sut.Save(firstInstance);

            secondInstance.Handle(new CreateCustomer(MessageId.New, "email@gmail.com"));
            var exception = await Should.ThrowAsync<DomainException>(async () => await _sut.Save(secondInstance));

            exception.Message
                .ShouldBe($"Customer {secondInstance.Id} was changed by another actor");
        }
    }
}
