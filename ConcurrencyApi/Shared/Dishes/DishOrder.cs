namespace ConcurrencyApi.ChannelsCode;

public class DishOrder
{
    public Guid OrderCode { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public required Guid DishCode { get; init; }
    public required Dish Dish { get; init; }

    public required string UserName { get; init; }
}