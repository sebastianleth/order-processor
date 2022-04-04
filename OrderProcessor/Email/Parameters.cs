using NodaTime;
using OrderProcessor.Domain;

namespace OrderProcessor.Email;

public record Parameters(
    string? Email,
    Domain.Order OrderPlaced,
    Domain.ILevel CustomerLevel,
    int OrderCount,
    decimal OrdersSum,
    Instant EarliestOrderTime,
    Instant TimeOfLastUpgrade)
{
    public static Parameters From(Customer customer, Order orderPlaced) => new(
        customer.State.Email,
        orderPlaced,
        customer.State.Level,
        customer.State.Orders.Length,
        customer.State.Orders.Sum(o => o.Total),
        customer.State.Orders.Min(o => o.Time),
        customer.State.LastLevelUp);
}