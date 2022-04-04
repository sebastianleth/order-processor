using NodaTime;

namespace OrderProcessor.Email;

public interface ISender
{
    Task SendEmail(Parameters parameters);
}