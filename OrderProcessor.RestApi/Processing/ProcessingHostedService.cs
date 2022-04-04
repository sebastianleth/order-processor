namespace OrderProcessor.Processing;

public class ProcessingHostedService : IHostedService
{
    readonly IProcessor _processor;

    public ProcessingHostedService(IProcessor processor)
    {
        _processor = processor;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _processor.Process(cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}