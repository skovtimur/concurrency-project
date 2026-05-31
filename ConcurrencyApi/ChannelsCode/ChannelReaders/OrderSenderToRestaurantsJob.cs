using System.Threading.Channels;

namespace ConcurrencyApi.ChannelsCode.ChannelReaders;

public class OrderSenderToRestaurantsJob(
    ILogger<OrderSenderToRestaurantsJob> logger,
    RestaurantsService restaurantsService,
    Channel<SendOrderToRestaurantsRequest> channel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await channel.Reader.WaitToReadAsync(stoppingToken))
            {
                var request = await channel.Reader.ReadAsync(stoppingToken);
                await Task.Delay(1000, stoppingToken); //kinda work

                var restaurantName = await restaurantsService.GetNearestOpenRestaurant(request.DishCode);

                logger.LogInformation(
                    "The order(id: {orderId}) with the dish code '{dishCode}' sent to the restaurant named '{name}'",
                    request.OrderCode, request.DishCode, restaurantName);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogCritical("Cancellation requested. Stopped {name} service.", nameof(OrderSenderToRestaurantsJob));
        }
    }
}