namespace OrderProcessor.Domain;

public record CustomerLevelResult(ICustomerLevel CustomerLevel, bool LevelBumped);