using System.Collections.Concurrent;

namespace OrderProcessor.Aggregates
{
    class InMemoryRepository : IAggregateRepository
    {
        readonly ConcurrentDictionary<Guid, AggregateData> _aggregates = new();

        public Task Save<T, TState>(T aggregate) where T : Aggregate<TState> where TState : new()
        {
            // Optimistic concurrency
            if (AggregateExists(aggregate.Id) && AggregateChanged(aggregate.Id, aggregate.Version))
            {
                throw new DomainException($"{aggregate.GetType().Name} {aggregate.Id} was changed by another actor");
            }

            WriteAggregateData<T, TState>(aggregate);

            return Task.CompletedTask;
        }

        public Task<T> Load<T, TState>(AggregateId aggregateId, Func<AggregateId, object, T> aggregateFactory) where T : Aggregate<TState> where TState : new()
        {
            if (!AggregateExists(aggregateId))
            {
                throw new DomainException($"{typeof(T).Name} {aggregateId} does not exist");
            }

            var aggregateData = _aggregates[aggregateId.Value];
            var state = (TState) aggregateData.State;
            var aggregate = aggregateFactory(aggregateId, state);

            return Task.FromResult(aggregate);
        }

        void WriteAggregateData<T, TState>(T aggregate) where T : Aggregate<TState> where TState : new()
        {
            _aggregates[aggregate.Id.Value] = new AggregateData(aggregate.State!, aggregate.Version);
        }

        bool AggregateChanged(AggregateId aggregateId, int newVersion) => _aggregates[aggregateId.Value].Version > newVersion;

        bool AggregateExists(AggregateId aggregateId)  => _aggregates.ContainsKey(aggregateId.Value);

        record AggregateData(object State, int Version);
    }
}
