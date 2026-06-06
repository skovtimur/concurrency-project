namespace ConcurrencyApi.ChannelsCode;

public class RestaurantsService
{
    public Task<string> GetNearestOpenRestaurant(Guid dishCode)
    {
        return Task.FromResult($"{dishCode}-" + Random.Shared.Next(0, 1000));
    }
}