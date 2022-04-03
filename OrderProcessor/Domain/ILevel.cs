namespace OrderProcessor.Domain;

public interface ILevel
{
    decimal Discount { get;  }

    // Perhaps more differentiated settings per customer level here, later?
}