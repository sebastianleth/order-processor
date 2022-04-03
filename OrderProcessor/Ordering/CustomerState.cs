namespace OrderProcessor.Ordering;

public record CustomerState
{
    public CustomerId Id { get; init; }
    public string Email { get; init; }
}