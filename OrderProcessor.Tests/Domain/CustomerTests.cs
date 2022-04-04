using System;
using NodaTime;
using OrderProcessor.Commands;
using OrderProcessor.Messaging;
using Shouldly;
using Xunit;

namespace OrderProcessor.Domain
{
    public class CustomerTests
    {
        [Fact]
        public void GivenCustomer_WhenCreate_ThenStatusRegular()
        {
            var time = Instant.MaxValue;
            var aggregateId = new CustomerId(Guid.NewGuid());
            var customer = Customer.Initialize(aggregateId, new CustomerState());

            customer.Handle(new CreateCustomer(MessageId.New, time, "sebastian@koderi.dk"));

            customer.State.CustomerLevel
                .ShouldBeAssignableTo<RegularLevel>();
        }
    }
}
