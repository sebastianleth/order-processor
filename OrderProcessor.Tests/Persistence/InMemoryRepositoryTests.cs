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
        readonly IAggregateRepository _sut = new InMemoryAggregateRepository();

        [Fact]
        public async Task GivenNotExistingAggregate_WhenLoad_ThenFail()
        {
            var missingAggregateId = CustomerId.FromEmail("sebastian@koderi.dk");

            var exception = await Should.ThrowAsync<DomainException>(async() => await _sut.Load<CustomerId, Customer, CustomerState>(missingAggregateId, Customer.Initialize));

            exception.Message
                .ShouldBe($"Customer {missingAggregateId} does not exist");
        }

        [Fact]
        public async Task GivenExistingAggregate_WhenLoad_ThenAggregateLoaded()
        {
            var aggregateId = CustomerId.FromEmail("sebastian@koderi.dk");
            var expected = Customer.Initialize(aggregateId, new CustomerState());
            await _sut.Save<CustomerId, Customer, CustomerState>(expected);

            var actual = await _sut.Load<CustomerId, Customer, CustomerState>(aggregateId, Customer.Initialize);

            actual.Id
                .ShouldBe(expected.Id);

            actual.State
                .ShouldBe(expected.State);
        }

        [Fact]
        public async Task GivenNotExistingAggregate_WhenSaveNew_ThenOk()
        {
            var newAggregateId = CustomerId.FromEmail("sebastian@koderi.dk");
            var customer = Customer.New(newAggregateId);

            await _sut.Save<CustomerId, Customer, CustomerState>(customer);
        }

        [Fact]
        public async Task GivenExistingAggregate_WhenSaveNew_ThenFailByAlreadyExisting()
        {
            var newAggregateId = CustomerId.FromEmail("sebastian@koderi.dk");
            var customer = Customer.New(newAggregateId);

            await _sut.Save<CustomerId, Customer, CustomerState>(customer);

            customer = Customer.New(newAggregateId);
            var exception = await Should.ThrowAsync<DomainException>(async () => await _sut.Save<CustomerId, Customer, CustomerState>(customer));

            exception.Message
                .ShouldBe($"Customer {customer.Id} already exists, and cannot be created anew");
        }

        [Fact]
        public async Task GivenExistingChangedAggregate_WhenSaveUnchangedSecondInstanceOfSameAggregate_ThenFailByOptimisticConcurrency()
        {
            var aggregateId = CustomerId.FromEmail("sebastian@koderi.dk");
            var time = Instant.MaxValue;
            var aggregate = Customer.Initialize(aggregateId, new CustomerState());
            await _sut.Save<CustomerId, Customer, CustomerState>(aggregate);

            var firstInstance = await _sut.Load<CustomerId, Customer, CustomerState>(aggregateId, Customer.Initialize);
            var secondInstance = await _sut.Load<CustomerId, Customer, CustomerState>(aggregateId, Customer.Initialize);

            firstInstance.Handle(new CreateCustomer(MessageId.New, time, "first@gmail.com"));
            await _sut.Save<CustomerId, Customer, CustomerState>(firstInstance);

            secondInstance.Handle(new CreateCustomer(MessageId.New, time, "first@gmail.com"));
            var exception = await Should.ThrowAsync<DomainException>(async () => await _sut.Save<CustomerId, Customer, CustomerState>(secondInstance));

            exception.Message
                .ShouldBe($"Customer {secondInstance.Id} was changed by another actor");
        }
    }
}
