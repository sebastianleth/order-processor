namespace OrderProcessor.Commands
{
    public record PlaceOrder(
        MessageId Id,
        decimal Total,
        string Email
    ) : Messaging.Message(Id), ICommand;
}