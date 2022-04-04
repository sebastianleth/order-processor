namespace OrderProcessor.Commands
{
    public record CreateCustomer(
        Messaging.MessageId Id,
        string Email
    ) : Messaging.Message(Id), ICommand;
}