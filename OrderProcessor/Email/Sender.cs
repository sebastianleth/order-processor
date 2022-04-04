
namespace OrderProcessor.Email;

class Sender : ISender
{
    private readonly Serilog.ILogger _logger;

    public Sender(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public Task SendEmail(string email, Domain.Order order, Domain.ICustomerLevel customerLevel)
    {
        var body = @"

            To:     {email}

            Your order has been placed.

            Total:  DKK {total}

            Your current and future level is {customerLevel}, that allows for a {discount}% discount!
        ";

        _logger.Information(body, email, order.Total, customerLevel.Name, customerLevel.Discount);

        return Task.CompletedTask;
    }
}