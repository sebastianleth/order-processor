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
            var clock = SystemClock.Instance;
            var aggregateId = new CustomerId(Guid.NewGuid());
            var customer = new Customer(aggregateId, new CustomerState(), clock);

            customer.Handle(new CreateCustomer(MessageId.New, "email@gmail.com"));

            customer.State.Level
                .ShouldBeAssignableTo<RegularLevel>();
        }
    }
}
