namespace OrderProcessor.Aggregates;

public interface IAggregateRepository
{
    Task Save<T, TState>(T aggregate) where T : Aggregate<TState> where TState : new();

    Task<T> Load<T, TState>(AggregateId aggregateId, Func<AggregateId, object, T> aggregateFactory) where T: Aggregate<TState> where TState : new();    
}