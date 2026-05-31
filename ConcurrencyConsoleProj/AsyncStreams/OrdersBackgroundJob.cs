using System.Runtime.CompilerServices;
using ConcurrencyConsoleProj.Shared;

namespace ConcurrencyConsoleProj.AsyncStreams;

public class OrdersBackgroundJob
{
    public async Task LaunchLiveStreamAsync()
    {
        try
        {
            ShippingService shippingService = new ShippingService();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            IAsyncEnumerable<Order> stream = GetOrdersAsync(cts.Token);

            Console.WriteLine("Waiting...");
            await Task.Delay(3000, cts.Token);

            //LINQ method for IAsyncEnumerable was taken from 'System.Linq.Async'
            stream = stream.Where(order => order.TotalPrice > 0);

            var firstOrder = await stream.FirstOrDefaultAsync(cancellationToken: cts.Token);
            if (firstOrder != null)
            {
                Console.WriteLine($"First order id: {firstOrder.OrderId}, Total price: {firstOrder.TotalPrice}");
            }

            // WithCancellation is used to put a cancellation token
            await foreach (var newOrder in stream
                               .WithCancellation(cts.Token)
                               .ConfigureAwait(false))
            {
                Console.WriteLine(
                    $"{newOrder.OrderId}: {newOrder.TotalPrice} | {newOrder.TotalPrice + shippingService.CalculateShippingCostsAsync(newOrder)}$");
            }

            throw new NotImplementedException();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Canceled!");
        }
    }

    //EnumeratorCancellation attribute make the compiler takes care of passing the token from WithCancellation 
    public async IAsyncEnumerable<Order> GetOrdersAsync([EnumeratorCancellation] CancellationToken ct = default,
        int? limit = null, int pauseMilliseconds = 1000, bool createZeroElement = true)
    {
        if (limit is <= 0)
        {
            throw new ArgumentException("Limit must be greater than 0");
        }

        if (createZeroElement)
        {
            yield return new Order
            {
                OrderId = 0,
                CustomerId = Guid.NewGuid(),
                PriceOfOne = 5000,
                Amount = 3
            };
        }

        var id = 1;

        while (ct.IsCancellationRequested == false && (limit is null or > 0))
        {
            await Task.Delay(pauseMilliseconds, ct);
            yield return OrderFactory.Create(ref id);
            limit--;
        }
    }
}