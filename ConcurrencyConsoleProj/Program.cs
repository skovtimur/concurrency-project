// Nuget Libraries:
// 1) System.Reactive for reactive programming
// 2) System.Threading.Collections for some additional concurrent collection
// 3) System.Collections.Immutable for some immutable collections
// 4) System.Threading.Tasks.DataFlow for DataFlow

using ConcurrencyConsoleProj.AsyncStreams;
using ConcurrencyConsoleProj.DataflowCode;
using ConcurrencyConsoleProj.EventHandlers;
using ConcurrencyConsoleProj.Shared;

namespace ConcurrencyConsoleProj;

public static class Program
{
    private enum WhatToDo
    {
        AsyncStreams,
        DataflowCode,
        EventHandlers,
    }

    public static async Task Main(string[] args)
    {
        var whatToDo = WhatToDo.EventHandlers;
        // var orders = OrderFactory.CreateOrders(1000, 0);

        switch (whatToDo)
        {
            case WhatToDo.AsyncStreams:
                //--- Launch an infinite (live) stream that will write new orders 
                // Async streams may be used in SignalR or EF Core for example. But an async stream taken from EFCore's IQueryable.AsAsyncEnumerable() is not infinite and just takes elements one by one instead all elements at once   
                // Important: async streams yield elements one by one, whereas a List<T> contains all elements in memory at once.
                await new OrdersBackgroundJob().LaunchLiveStreamAsync();
                break;

            case WhatToDo.DataflowCode:
                var ordersToValidate = new OrdersBackgroundJob()
                    .GetOrdersAsync(limit: 999, pauseMilliseconds: 3, createZeroElement: true);

                await OrderValidator.Validate(ordersToValidate);
                break;


            case WhatToDo.EventHandlers:
                var player = new Player(100, 100);
                var uiSystem = new UiSystem();
                var alliesSystem = new AlliesSystem();

                player.OnDamaged += uiSystem.PrintDamage;
                player.OnDamaged += uiSystem.PrintGameOverTitle;
                player.OnDamaged += alliesSystem.AnalyzePlayerNeedAHealer;
                player.OnDamaged += (sender, eventArgs) => { Logger.Log($"Damage: {eventArgs.Damage}"); };

                player.TakeDamage(15);
                player.TakeDamage(10);
                player.TakeDamage(45);
                player.TakeDamage(41);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        Logger.Log("Press any key to exit...", ConsoleColor.Cyan);
        Console.ReadKey();
    }
}