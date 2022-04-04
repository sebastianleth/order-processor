namespace OrderProcessor.Domain;

class SilverLevel : ICustomerLevel
{
    public string Name => "Silver level";
    public decimal Discount => 10;
}