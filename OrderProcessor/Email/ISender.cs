namespace OrderProcessor.Email;

public interface ISender
{
    Task SendEmail(string email, Domain.Order orderPlaced, Domain.ICustomerLevel customerLevel);
}