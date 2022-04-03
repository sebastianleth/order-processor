namespace OrderProcessor.Domain;

public static class Discount
{
    public static decimal Calculate(CustomerStatus status)
    {
        return status switch
        {
            CustomerStatus.Regular => 0,
            CustomerStatus.Silver => 10,
            CustomerStatus.Gold => 15,
            _ => throw new NotImplementedException("Unsupported customer status in discount calculation")
        };
    }
}