namespace OrderProcessor.Domain;

public interface ICustomerLevel
{
    string Name { get; }
    decimal DiscountPercentage { get;  }

    // Perhaps more differentiated settings per customer level here, later?
}