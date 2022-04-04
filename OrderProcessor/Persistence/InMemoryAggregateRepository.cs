using System.Collections.Concurrent;

namespace OrderProcessor.Persistence
{
    class InMemoryAggregateRepository : IAggregateRepository
    {
        readonly ConcurrentDictionary<Guid, object> _aggregates = new();

        public Task Insert<TAggregateId, T, TState>(T aggregate)
            where TAggregateId : AggregateId
            where T : Aggregate<TState>
            where TState : AggregateState, new()
        {
            if (AggregateExists(aggregate.Id))
            {
                throw new DomainException($"{aggregate.GetType().Name} {aggregate.Id} already exists, and cannot be created anew");
            }

            WriteAggregateData<T, TState>(aggregate);

            return Task.CompletedTask;
        }

        public Task Save<TAggregateId, T, TState>(T aggregate)
            where TAggregateId : AggregateId
            where T : Aggregate<TState> 
            where TState : AggregateState, new()
        {
            if (AggregateExists(aggregate.Id) && AggregateChanged<TState>(aggregate.Id, aggregate.State.Version))
            {
                throw new DomainException($"{aggregate.GetType().Name} {aggregate.Id} was changed by another actor");
            }

            WriteAggregateData<T, TState>(aggregate);

            return Task.CompletedTask;
        }

        public Task<T> Load<TAggregateId, T, TState>(TAggregateId aggregateId, Func<TAggregateId, TState, T> aggregateFactory)
            where TAggregateId : AggregateId
            where T : Aggregate<TState> 
            where TState : AggregateState, new()
        {
            if (!AggregateExists(aggregateId))
            {
                throw new DomainException($"{typeof(T).Name} {aggregateId} does not exist");
            }

            var state = GetState<TState>(aggregateId);
            var aggregate = aggregateFactory(aggregateId, state);

            return Task.FromResult(aggregate);
        }

        void WriteAggregateData<T, TState>(T aggregate) 
            where T : Aggregate<TState> 
            where TState : AggregateState, new()
        {
            _aggregates[aggregate.Id.Value] = aggregate.State!;
        }

        bool AggregateChanged<TState>(AggregateId aggregateId, int newVersion) where TState : AggregateState => 
            GetState<TState>(aggregateId).Version >= newVersion;

        bool AggregateExists(AggregateId aggregateId)  => _aggregates.ContainsKey(aggregateId.Value);

        TState GetState<TState>(AggregateId aggregateId) => (TState)_aggregates[aggregateId.Value];
    }
}
