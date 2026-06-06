namespace ConcurrencyApi.ChannelsCode;

public class OrderReceiptRequest
{
    public required Guid OrderCode { get; init; }
    public required Guid DishCode { get; init; }
    public required Dish Dish { get; init; }
    public required string UserName { get; init; }
}