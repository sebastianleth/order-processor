namespace OrderProcessor.Domain;

public static class Levels
{
    public static ILevel Regular => new RegularLevel();
    public static ILevel Silver => new SilverLevel();
    public static ILevel Gold => new GoldLevel();
}