using System.Threading.Tasks.Dataflow;
using ConcurrencyConsoleProj.Shared;

namespace ConcurrencyConsoleProj.DataflowCode;

public static class OrderValidator
{
    public static async Task Validate(IAsyncEnumerable<Order> ordersToValidate,
        bool todayIsDayForDiscounts = true,
        bool throwException = false)
    {
        // 1) TransformManyBlock:
        var linkOptions = new DataflowLinkOptions
        {
            PropagateCompletion = true,
            //max messages count is unlimited:
            MaxMessages = DataflowBlockOptions.Unbounded,
        };
        var dataValidatorBlock = new TransformManyBlock<Order, Order>(x =>
        {
            if (x.PriceOfOne <= 0 ||
                x.TotalPrice <= 0 ||
                x.Amount < 1)
            {
                Logger.LogError($"Invalid order {x.OrderId}");
                return [];
            }

            if (throwException)
            {
                throw new Exception("EXCEPTED EXCEPTION!");
            }

            return [x];
        });

        //2) TransformBlock
        var discountSaverBlock = new TransformBlock<Order, Order>(x =>
        {
            if (todayIsDayForDiscounts == false)
            {
                return x;
            }

            // make discounts randomly on the 10% of orders, I know, that's stupid, because it must make discounts on PRODUCTS but not on orders
            var needToMakeDiscount = Random.Shared.NextDouble() < 0.1;

            if (needToMakeDiscount == false)
            {
                return x;
            }

            var percent = GetRandomPercent();
            x.DiscountPercentage = percent;

            // saving to db....

            return x;
        }, new ExecutionDataflowBlockOptions
        {
            //how much messages the block can handle in parallel at a time, DataflowBlockOptions.Unbounded - infinity
            MaxDegreeOfParallelism = 3
        });


        // 3) Unlinking:
        var uselessBlock = new TransformBlock<Order, Order>(x => x);
        IDisposable uselessLink = dataValidatorBlock.LinkTo(uselessBlock, linkOptions);
        uselessLink.Dispose(); //in that way it deletes the link

        // 4) ActionBlock
        var outputBlocksOptions = new ExecutionDataflowBlockOptions
        {
            //By default(BoundedCapacity = -1, unlimited), the first block will take all the elements, but the second will take nothing
            //BoundedCapacity = 5 make the both output blocks take only 5 element at a time.
            // ----BoundedCapacity is a limit of elements for a block (it's not a load balancer, but it's a limit)-----
            BoundedCapacity = 5,

            // Example (BoundedCapacity = 4): 0 1 2 3 4 5 6 7 8 9
            // First will take: 0, 1, 2, 3
            // Second will take: 4, 5, 6, 7
            // So if the one of them vacates a space, it will take 8 and\or 9
            // Sure if each element takes a lot of time, otherwise the first can process all the messages

            MaxDegreeOfParallelism = 3
        };
        var shippingService = new ShippingService();

        async Task OutputAction(Order x, string index)
        {
            await Task.Delay(100);
            //It need to wait to let the second block take elements until the first queue is full,
            //otherwise the first will handle messages immediately

            var totalPriceText = $"{x.TotalPrice}";

            if (x.DiscountPercentage > 0)
            {
                totalPriceText = $"{x.TotalPriceWithDiscount} / {x.TotalPrice}";
            }

            Logger.Log(
                $"{index} {x.OrderId}) quantity: {x.Amount}, total price: {totalPriceText}" +
                $" + {shippingService.CalculateShippingCostsAsync(x)} for shipping");
        }

        var firstOutputBlock = new ActionBlock<Order>(async x => await OutputAction(x, "FIRST "), outputBlocksOptions);
        var secondOutputBlock = new ActionBlock<Order>(async x => await OutputAction(x, "SECOND"), outputBlocksOptions);
        // if you want all the blocks process the element then use BroadcastBlock
        // In contrast to TransformBlock, BroadcastBlock puts its each element on its each linked blocks
        // TransformBlock puts an element on the one of the linked blocks 

        // 5) Linking:
        dataValidatorBlock.LinkTo(discountSaverBlock, linkOptions);
        discountSaverBlock.LinkTo(firstOutputBlock, linkOptions);
        discountSaverBlock.LinkTo(secondOutputBlock, linkOptions);

        // 6) Send messages:
        await foreach (var order in ordersToValidate)
        {
            //bool resultOfPost = dataValidatorBlock.Post(order);
            //Post method trynna synchronously push new messages immediately to blocks, it works like fire-and-forget
            //Post returns false when BoundedCapacity is already full


            var resultOfAsync = await dataValidatorBlock
                .SendAsync(order); //SendAsync is asynchronous and put a new message to something like a queue
            //SendAsync will wait for BoundedCapacity is free to put new messages 
        }

        try
        {
            // 7)
            //Complete() method signals a block to stop accepting new messages (it doesn't stop handling the current messages)
            dataValidatorBlock.Complete();


            // Wait for the blocks are completed:
            await Task.WhenAll(
                firstOutputBlock.Completion,
                secondOutputBlock.Completion);

            Logger.Log("Completed");
        }
        catch (AggregateException ex)
        {
            var originalEx = ex.Flatten();

            Logger.LogError($"Exception Message({ex.GetType()}): {ex.Message}");
            Logger.LogError($"Original Exception Message({ex.GetType()}): {originalEx.Message}");
        }
    }

    private static int GetRandomPercent(int start = 0, int end = 99)
    {
        return Random.Shared.Next(start, end);
    }
}