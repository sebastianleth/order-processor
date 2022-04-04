namespace OrderProcessor.Persistence;

public abstract record AggregateState
{
    public int Version { get; init;  } = -1;
}