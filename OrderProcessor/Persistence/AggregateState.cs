namespace OrderProcessor.Persistence;

public abstract record AggregateState(int Version = -1);