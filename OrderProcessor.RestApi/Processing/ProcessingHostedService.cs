namespace OrderProcessor.Processing;

public class ProcessingHostedService : IHostedService
{
    readonly IProcessor _processor;

    public ProcessingHostedService(IProcessor processor)
    {
        _processor = processor;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _processor.Process(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}