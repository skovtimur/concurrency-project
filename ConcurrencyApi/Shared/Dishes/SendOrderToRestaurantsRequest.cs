namespace ConcurrencyApi.ChannelsCode;

public class SendOrderToRestaurantsRequest
{
    public required Guid OrderCode { get; init; }
    public required Guid DishCode { get; init; }
    public required string UserName { get; init; }
}