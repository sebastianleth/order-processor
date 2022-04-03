namespace OrderProcessor.Email;

public interface ISender
{
    Task SendEmail(string email);
}