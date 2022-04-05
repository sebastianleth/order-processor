namespace OrderProcessor.Commands
{
    public record CreateCustomer(
        MessageId Id,
        string Email
    ) : Messaging.Message(Id), ICommand;
}