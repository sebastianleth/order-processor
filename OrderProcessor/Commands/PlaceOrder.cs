namespace OrderProcessor.Commands
{
    public record PlaceOrder(
        Messaging.MessageId Id,
        decimal Total,
        string Email
    ) : Messaging.Message(Id), ICommand;
}