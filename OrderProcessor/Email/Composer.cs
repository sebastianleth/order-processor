namespace OrderProcessor.Email;

class Composer : IComposer
{
    public Email Do(Parameters parameters)
    {
        var body = $@"

                To:     {parameters.Email}

                Your order has been placed.

                Total:      DKK {parameters.OrderPlaced.Total}
                Discount:   DKK {parameters.OrderPlaced.DiscountGiven}

                You have placed {parameters.OrderCount} orders for a total of DKK {parameters.OrdersSum} since {parameters.EarliestOrderTime}.

                Your customer level is {parameters.Level.Name} since {parameters.LastLevelUpTime}, which allows for a {parameters.Level.DiscountPercentage}% discount!
        ";

        return new Email(body);
    }
}