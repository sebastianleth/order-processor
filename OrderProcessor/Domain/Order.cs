using NodaTime;

namespace OrderProcessor.Domain;

public record Order(
    OrderId Id,
    Instant Time,
    decimal Total,
    decimal DiscountGiven)
{
    public static Order Create(OrderId orderId, decimal total, ILevel level, Instant now)
    {
        var discountGiven = (level.DiscountPercentage / 100) * total;
        var totalAfterDiscount = total - discountGiven;

        return new Order(orderId, now, totalAfterDiscount, discountGiven);
    }
}