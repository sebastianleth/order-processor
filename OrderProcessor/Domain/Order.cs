using NodaTime;

namespace OrderProcessor.Domain;

public record Order(
    OrderId Id,
    Instant Time,
    decimal Total,
    decimal DiscountGiven)
{
    public static Order Create(OrderId orderId, decimal total, Instant now) =>
        new (orderId, now, total, DiscountGiven: 0);

    public Order ApplyDiscount(ILevel level)
    {
        var discountGiven = (level.DiscountPercentage / 100) * Total;
        var totalAfterDiscount = Total - discountGiven;

        return this with
        {
            Total = totalAfterDiscount,
            DiscountGiven = discountGiven
        };
    }
}