using System.Collections.Immutable;

namespace OrderProcessor.Domain;

public record Order(OrderId Id, ImmutableArray<OrderLine> Lines);