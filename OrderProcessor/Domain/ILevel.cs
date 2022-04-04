﻿using NodaTime;

namespace OrderProcessor.Domain;

public interface ILevel
{
    string Name { get; }
    decimal DiscountPercentage { get;  }

    LevelResult DetermineLevelUp(CustomerState state, Instant now);
}