using System.Collections.Concurrent;

namespace OrderProcessor.Persistence
{
    class InMemoryAggregateRepository<TAggregateId, T, TState> : IAggregateRepository<TAggregateId, T>
        where TAggregateId : AggregateId
        where T : Aggregate<TState>
        where TState : AggregateState, new()
    {
        readonly Func<TAggregateId, TState, T> _aggregateFactory;
        readonly ConcurrentDictionary<Guid, object> _aggregates = new();

        public InMemoryAggregateRepository(Func<TAggregateId, TState, T> aggregateFactory)
        {
            _aggregateFactory = aggregateFactory;
        }

        public Task<T> New(TAggregateId aggregateId)
        {
            var state = Activator.CreateInstance<TState>();
            var aggregate = _aggregateFactory(aggregateId, state);

            return Task.FromResult(aggregate);
        }

        public Task Insert(T aggregate)
        {
            if (AggregateExists(aggregate.Id))
            {
                throw new DomainException($"{aggregate.GetType().Name} {aggregate.Id} already exists, and cannot be created anew");
            }

            WriteAggregateData(aggregate);

            return Task.CompletedTask;
        }

        public Task Save(T aggregate)
        {
            if (AggregateExists(aggregate.Id) && AggregateChanged(aggregate.Id, aggregate.State.Version))
            {
                throw new DomainException($"{aggregate.GetType().Name} {aggregate.Id} was changed by another actor");
            }

            WriteAggregateData(aggregate);

            return Task.CompletedTask;
        }

        public Task<T> Load(TAggregateId aggregateId)
        {
            if (!AggregateExists(aggregateId))
            {
                throw new DomainException($"{typeof(T).Name} {aggregateId} does not exist");
            }

            var state = GetState(aggregateId);
            var aggregate = _aggregateFactory(aggregateId, state);

            return Task.FromResult(aggregate);
        }

        void WriteAggregateData(T aggregate)
        {
            _aggregates[aggregate.Id.Value] = aggregate.State!;
        }

        bool AggregateChanged(AggregateId aggregateId, int newVersion) => 
            GetState(aggregateId).Version >= newVersion;

        bool AggregateExists(AggregateId aggregateId)  => _aggregates.ContainsKey(aggregateId.Value);

        TState GetState(AggregateId aggregateId) => (TState)_aggregates[aggregateId.Value];
    }
}
