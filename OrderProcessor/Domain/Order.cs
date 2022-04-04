using NodaTime;

namespace OrderProcessor.Domain;

public record Order(
    OrderId Id, 
    Instant Time,
    decimal Total,
    decimal DiscountGiven);