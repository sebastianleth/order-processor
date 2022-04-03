namespace OrderProcessor.Domain;

public interface ICustomerLevel
{
    decimal Discount { get;  }

    // Perhaps more differentiated settings per customer level here, later?
}