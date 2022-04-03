namespace OrderProcessor.Aggregates;

public interface IAggregateRepository
{
    Task Save<TAggregateId, T, TState>(T aggregate)
        where TAggregateId : AggregateId
        where T : Aggregate<TState> 
        where TState : AggregateState, new();

    Task<T> Load<TAggregateId, T, TState>(TAggregateId aggregateId, Func<TAggregateId, TState, T> aggregateFactory)
        where TAggregateId: AggregateId
        where T: Aggregate<TState> 
        where TState : AggregateState, new();    
}