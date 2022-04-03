namespace OrderProcessor.Ordering
{
    public record CreateCustomerCommand(
        Messaging.MessageId Id,
        string Email
    ) : Messaging.Message(Id);
}