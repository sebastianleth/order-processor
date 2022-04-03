namespace OrderProcessor.Domain
{
    public record CreateCustomerCommand(
        Messaging.MessageId Id,
        string Email
    ) : Messaging.Message(Id);
}