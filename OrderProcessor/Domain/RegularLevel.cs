namespace OrderProcessor.Domain;

class RegularLevel : ICustomerLevel
{
    public string Name => "Regular level";
    public decimal Discount => 0;
}