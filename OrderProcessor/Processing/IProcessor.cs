namespace OrderProcessor.Processing;

public interface IProcessor
{
    Task Process(CancellationToken cancellationToken);
}