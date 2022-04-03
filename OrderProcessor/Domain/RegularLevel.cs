namespace OrderProcessor.Domain;

class RegularLevel : ICustomerLevel
{
    public decimal Discount => 0;
}