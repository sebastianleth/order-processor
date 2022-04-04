namespace OrderProcessor.Domain;

public record LevelResult(ILevel NextLevel, bool LevelUp);