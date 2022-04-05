
using NodaTime;

namespace OrderProcessor.Email;

class Sender : ISender
{
    private readonly Serilog.ILogger _logger;

    public Sender(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public Task SendEmail(Email email)
    {
        _logger.Information("E-mail sent: {Body}", email.Body);

        return Task.CompletedTask;
    }
}