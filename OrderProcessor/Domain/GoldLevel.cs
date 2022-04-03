namespace OrderProcessor.Domain;

class GoldLevel : ICustomerLevel
{
    public decimal Discount => 15;
}