using Newtonsoft.Json;
using Serilog;

namespace OrderProcessor.Persistence;

public abstract class Aggregate<TState> where TState : AggregateState, new()
{
    readonly ILogger _logger;

    protected Aggregate(AggregateId id, TState state, ILogger logger)
    {
        _logger = logger;
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

        _logger.Information("New state applied to {Aggregate} {Id}: {State}", this.GetType().Name, Id.Value, JsonConvert.SerializeObject(State, Formatting.Indented));
    }

    bool Exists() => State.Version > -1;
}