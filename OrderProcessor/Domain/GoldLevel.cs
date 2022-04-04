namespace OrderProcessor.Domain;

class GoldLevel : ICustomerLevel
{
    public string Name => "Gold level";
    public decimal Discount => 15;
}