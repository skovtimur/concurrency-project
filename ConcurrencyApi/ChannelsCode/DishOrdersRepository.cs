using System.Collections.Concurrent;

namespace ConcurrencyApi.ChannelsCode;

public class DishOrdersRepository(DishRepository dishRepository)
{
    public async Task<DishOrder> CreateOrder(Guid dishCode, string userName)
    {
        var dish = await dishRepository.GetDishByCode(dishCode);
        var newDishOrder = new DishOrder
        {
            DishCode = dishCode,
            Dish = dish,
            UserName = userName,
        };

        Orders.Add(newDishOrder);
        return newDishOrder;
    }

    private static readonly ConcurrentBag<DishOrder> Orders = [];
}