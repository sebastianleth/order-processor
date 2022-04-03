namespace OrderProcessor.Domain;

class GoldLevel : CustomerLevel
{
    public override string Name => "Gold";
    public override decimal Discount => 15;
}