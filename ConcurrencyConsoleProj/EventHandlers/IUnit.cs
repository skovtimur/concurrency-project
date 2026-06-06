namespace ConcurrencyConsoleProj.EventHandlers;

public interface IUnit
{
    public int Health { get; }
    public bool IsDead { get; }
}