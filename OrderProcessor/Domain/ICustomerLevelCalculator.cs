namespace OrderProcessor.Domain;

public interface ICustomerLevelCalculator
{
    CustomerLevelResult Determine(CustomerState customerState);
}