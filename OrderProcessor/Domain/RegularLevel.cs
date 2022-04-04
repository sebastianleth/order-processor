namespace OrderProcessor.Domain;

class RegularLevel : ICustomerLevel
{
    public string Name => "Regular";
    public decimal DiscountPercentage => 0;
}