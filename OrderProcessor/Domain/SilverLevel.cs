namespace OrderProcessor.Domain;

class SilverLevel : ICustomerLevel
{
    public decimal Discount => 10;
}