//using Shouldly;
//using Xunit;

//namespace OrderProcessor.Domain
//{
//    public class DiscountTests
//    {
//        [Fact]
//        public void GivenRegularCustomer_WhenCalculateDiscount_ThenZero()
//        {
//            Discount.Calculate(CustomerStatus.Regular)
//                .ShouldBe(0);
//        }

//        [Fact]
//        public void GivenSilverCustomer_WhenCalculateDiscount_ThenTen()
//        {
//            Discount.Calculate(CustomerStatus.Silver)
//                .ShouldBe(10);
//        }

//        [Fact]
//        public void GivenGoldCustomer_WhenCalculateDiscount_ThenFifteen()
//        {
//            Discount.Calculate(CustomerStatus.Gold)
//                .ShouldBe(15);
//        }
//    }
//}
