namespace OrderProcessor.Persistence;

public abstract class Aggregate<TState> where TState : AggregateState, new()
{
    protected Aggregate(AggregateId id, TState state)
    {
        Id = id;
        State = state;
    }

    public AggregateId Id { get; }

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
        State = state with { Version = state.Version + 1 };
    }

    bool Exists() => State.Version > -1;
}