namespace OrderProcessor.Domain;

class SilverLevel : ICustomerLevel
{
    public string Name => "Silver";
    public decimal DiscountPercentage => 10;
}