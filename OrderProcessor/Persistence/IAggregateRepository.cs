namespace OrderProcessor.Persistence;

public interface IAggregateRepository<in TAggregateId, T>
{
    Task<T> New(TAggregateId aggregateId);

    Task Insert(T aggregate);

    Task Save(T aggregate);

    Task<T> Load(TAggregateId aggregateId);
}