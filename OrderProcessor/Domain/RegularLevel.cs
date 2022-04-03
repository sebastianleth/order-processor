namespace OrderProcessor.Domain;

class RegularLevel : CustomerLevel
{
    public override string Name => "Regular";
    public override decimal Discount => 0;
}