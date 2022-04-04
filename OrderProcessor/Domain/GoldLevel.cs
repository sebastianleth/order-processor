namespace OrderProcessor.Domain;

class GoldLevel : ICustomerLevel
{
    public string Name => "Gold";
    public decimal DiscountPercentage => 15;
}