namespace OrderProcessor.Persistence;

public abstract class Aggregate<TState> where TState : new()
{
    protected Aggregate(AggregateId id, TState state)
    {
        Id = id;
        State = state;
    }

    public AggregateId Id { get; }

    public int Version { get; private set; } = -1;

    public TState State { get; private set; }

    public void EnsureExists()
    {
        if (!Exists())
        {
            throw new DomainException($"{GetType().Name} {Id} does not exist");
        }
    }

    public void EnsureDoesNotExist()
    {
        if (Exists())
        {
            throw new DomainException($"{GetType().Name} {Id} already exists");
        }
    }

    protected void ApplyState(TState state)
    {
        State = state;
        Version++;
    }

    bool Exists() => Version > -1;
}