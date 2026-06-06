using ConcurrencyConsoleProj.Shared;

namespace ConcurrencyConsoleProj.EventHandlers;

public class UiSystem
{
    public void PrintDamage(object? sender, PlayerDamagedEventArgs args)
    {
        //sender is a player object
        Logger.LogWarn($"UI damage: {args.Damage}. Remaining health: {args.RemainingHealth} HP");
    }

    public void PrintGameOverTitle(object? sender, PlayerDamagedEventArgs args)
    {
        if (args.IsDead)
        {
            Logger.Log("DIED", ConsoleColor.DarkRed);
        }
    }
}