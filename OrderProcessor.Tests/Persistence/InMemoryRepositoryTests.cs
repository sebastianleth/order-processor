using System.Threading.Tasks;
using NodaTime;
using OrderProcessor.Commands;
using OrderProcessor.Domain;
using Shouldly;
using Xunit;

namespace OrderProcessor.Persistence
{
    public class InMemoryRepositoryTests
    {
        const string CustomerEmail = "email@gmail.com";
        static Customer CustomerFactory(CustomerId id, CustomerState state) => new(id, state, SystemClock.Instance, Serilog.Log.Logger);
        readonly IAggregateRepository<CustomerId, Customer> _sut = new InMemoryAggregateRepository<CustomerId, Customer, CustomerState>(CustomerFactory);

        [Fact]
        public async Task GivenNotExistingAggregate_WhenLoad_ThenFail()
        {
            var missingCustomerId = CustomerId.FromEmail(CustomerEmail);

            var exception = await Should.ThrowAsync<DomainException>(async() => await _sut.Load(missingCustomerId));

            exception.Message
                .ShouldBe($"Customer {missingCustomerId} does not exist");
        }

        [Fact]
        public async Task GivenExistingAggregate_WhenLoad_ThenAggregateLoaded()
        {
            var customerId = CustomerId.FromEmail(CustomerEmail);
            var customer = await _sut.New(customerId);
            await _sut.Insert(customer);

            var actual = await _sut.Load(customerId);

            actual.Id
                .ShouldBe(customer.Id);

            actual.State
                .ShouldBe(customer.State);
        }

        [Fact]
        public async Task GivenNotExistingAggregate_WhenInsertNew_ThenOk()
        {
            var newAggregateId = CustomerId.FromEmail(CustomerEmail);
            var customer = await _sut.New(newAggregateId);

            await _sut.Save(customer);
        }

        [Fact]
        public async Task GivenExistingAggregate_WhenInsertNew_ThenFailByAlreadyExisting()
        {
            var customerId = CustomerId.FromEmail(CustomerEmail);
            var customer = await _sut.New(customerId);

            await _sut.Insert(customer);

            customer = await _sut.New(customerId);
            var exception = await Should.ThrowAsync<DomainException>(async () => await _sut.Insert(customer));

            exception.Message
                .ShouldBe($"Customer {customer.Id} already exists, and cannot be created anew");
        }

        [Fact]
        public async Task GivenExistingChangedAggregate_WhenSaveUnchangedSecondInstanceOfSameAggregate_ThenFailByOptimisticConcurrency()
        {
            var customerId = CustomerId.FromEmail(CustomerEmail);
            var customer = await _sut.New(customerId);
            await _sut.Save(customer);

            var firstInstance = await _sut.Load(customerId);
            var secondInstance = await _sut.Load(customerId);

            firstInstance.Create(new CreateCustomer(MessageId.New, CustomerEmail));
            await _sut.Save(firstInstance);

            secondInstance.Create(new CreateCustomer(MessageId.New, CustomerEmail));
            var exception = await Should.ThrowAsync<DomainException>(async () => await _sut.Save(secondInstance));

            exception.Message
                .ShouldBe($"Customer {secondInstance.Id} was changed by another actor");
        }
    }
}
