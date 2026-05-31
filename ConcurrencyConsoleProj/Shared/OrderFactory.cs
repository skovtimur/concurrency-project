namespace ConcurrencyConsoleProj.Shared;

public static class OrderFactory
{
    private static readonly Random Random = new();

    public static Order Create(ref int id, bool enableAutoIncrement = true)
    {
        var order = new Order
        {
            OrderId = id,
            CustomerId = Guid.NewGuid(),
            PriceOfOne = Random.Next(10, 5000),
            Amount = Random.Next(1, 10)
        };

        if (enableAutoIncrement)
            id++;

        return order;
    }


    public static List<Order> CreateOrders(int numberOfOrders, int idOfFirstOrder)
    {
        if (numberOfOrders <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfOrders), numberOfOrders,
                "The number of orders must be greater than zero.");
        }

        if (idOfFirstOrder < 0)
        {
            throw new ArgumentException("The Id of the first order must be greater or equals zero.",
                nameof(idOfFirstOrder));
        }

        var list = new List<Order>();

        while (numberOfOrders > 0)
        {
            list.Add(Create(ref idOfFirstOrder, enableAutoIncrement: true));
            numberOfOrders--;
        }

        return list;
    }
}