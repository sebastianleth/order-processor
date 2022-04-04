namespace OrderProcessor.Persistence;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}