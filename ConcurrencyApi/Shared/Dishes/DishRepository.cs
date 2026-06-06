namespace ConcurrencyApi.ChannelsCode;

public class DishRepository
{
    public Task<Dish> GetDishByCode(Guid dishCode)
    {
        return Task.FromResult(new Dish
        {
            DishCode = dishCode,
            Name = Random.Shared.Next().ToString(),
            Description = Random.Shared.Next().ToString(),
            Price = Random.Shared.Next()
        });
    }
}