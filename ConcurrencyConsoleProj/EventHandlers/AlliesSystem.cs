using ConcurrencyConsoleProj.Shared;

namespace ConcurrencyConsoleProj.EventHandlers;

public class AlliesSystem
{
    private const int PercentToHelp = 50;

    public void AnalyzePlayerNeedAHealer(object? sender, PlayerDamagedEventArgs args)
    {
        if (args.IsDead)
        {
            return;
        }

        var percent = (args.RemainingHealth / args.MaxHp) * 100;

        if (percent <= PercentToHelp)
        {
            Logger.Log("Allies are gonna treat the player", ConsoleColor.Cyan);
        }
    }
}