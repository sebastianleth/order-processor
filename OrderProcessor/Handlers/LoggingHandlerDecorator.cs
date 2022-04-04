using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace OrderProcessor.Handlers;

public class LoggingHandlerDecorator<T>: ICommandHandler<T> where T : Commands.ICommand
{
    readonly ICommandHandler<T> _innerHandler;
    readonly Serilog.ILogger _logger;

    public LoggingHandlerDecorator(
        ICommandHandler<T> innerHandler,
        Serilog.ILogger logger)
    {
        _innerHandler = innerHandler;
        _logger = logger;
    }

    public Task Handle(T command)
    {
        _logger.Information("Handling command {commandType}: {command}", command.GetType(), JsonConvert.SerializeObject(command));

        return _innerHandler.Handle(command);
    }
}