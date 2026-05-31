using System.Threading.Channels;

namespace ConcurrencyApi.ChannelsCode.ChannelReaders;

public class OrderReceiptsCreatorJob(
    ILogger<OrderReceiptsCreatorJob> logger,
    Channel<OrderReceiptRequest> channel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // WaitToRead returns false if the channel is closed (by channel.Writer.Complete() or channel.Writer.TryComplete())
            // If the reader have some requests, it returns true, otherwise it will wait for new requests

            while (await channel.Reader.WaitToReadAsync(stoppingToken))
            {
                var request = await channel.Reader.ReadAsync(stoppingToken);
                await Task.Delay(1000, stoppingToken); //kinda work

                logger.LogInformation("A PDF receipt of the order '{orderCode}' has been created",
                    request.OrderCode);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogCritical("Cancellation requested. Stopped {name} service.", nameof(OrderReceiptsCreatorJob));
        }
    }
}