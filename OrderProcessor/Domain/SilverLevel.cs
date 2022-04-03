namespace OrderProcessor.Domain;

class SilverLevel : CustomerLevel
{
    public override string Name => "Silver";
    public override decimal Discount => 10;
}