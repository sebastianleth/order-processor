namespace OrderProcessor.Handlers;

public interface ICommandHandler<in TCommand> where TCommand : Commands.ICommand
{
    Task Handle(TCommand command);
}