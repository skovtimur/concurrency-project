namespace ConcurrencyConsoleProj.Shared;

public class Order
{
    public required int OrderId { get; init; }
    public required Guid CustomerId { get; init; }
    public required int Amount { get; init; } = 1;
    public required decimal PriceOfOne { get; init; }
    public decimal TotalPrice => PriceOfOne * Amount;

    public decimal TotalPriceWithDiscount =>
        DiscountPercentage == 0 ? TotalPrice : TotalPrice * DiscountPercentage / 100;

    public int DiscountPercentage
    {
        get => _discountPercentage;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Discount percentage cannot be less than zero.");
            }

            if (value >= 100)
            {
                throw new ArgumentException("Discount percentage cannot be greater or equals 100.");
            }

            _discountPercentage = value;
        }
    }

    private int _discountPercentage;
}