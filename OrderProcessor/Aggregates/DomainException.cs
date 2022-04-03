namespace OrderProcessor.Aggregates;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}