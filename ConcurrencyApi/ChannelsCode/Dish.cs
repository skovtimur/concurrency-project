namespace ConcurrencyApi.ChannelsCode;

public class Dish
{
    public required Guid DishCode { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
}