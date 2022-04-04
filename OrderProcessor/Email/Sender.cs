
using NodaTime;

namespace OrderProcessor.Email;

class Sender : ISender
{
    private readonly Serilog.ILogger _logger;

    public Sender(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public Task SendEmail(EmailParameters parameters)
    {
        var body = @"

            To:     {email}

            Your order has been placed.

            Total:      DKK {total}
            Discount:   DKK {discount}

            You have placed {orderCount} orders for a total of DKK {ordersSum} since {since}.

            Your customer level is {customerLevel} since {upgradeTime}, which allows for a {discountPercentage}% discount!
        ";

        _logger.Information(
            body, 
            parameters.Email, 
            parameters.OrderPlaced.Total,
            parameters.OrderPlaced.DiscountGiven,
            parameters.OrderCount,
            parameters.OrdersSum,
            parameters.EarliestOrderTime,
            parameters.CustomerLevel.Name, 
            parameters.TimeOfLastUpgrade,
            parameters.CustomerLevel.DiscountPercentage);

        return Task.CompletedTask;
    }
}