namespace ConcurrencyConsoleProj.Shared;

public class ShippingService
{
    private const decimal PercentForShipping = 0.05m;

    public decimal CalculateShippingCostsAsync(Order order)
    {
        return order.TotalPrice * PercentForShipping;
    }
}