
namespace OrderProcessor.Email;

class Sender : ISender
{
    private readonly Serilog.ILogger _logger;

    public Sender(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public Task SendEmail(string email)
    {
        _logger.Information("Email sent: {email}", email);

        return Task.CompletedTask;
    }
}