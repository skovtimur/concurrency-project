namespace ConcurrencyConsoleProj.EventHandlers;

public class PlayerDamagedEventArgs(int damage, bool wasDead, int maxHp, int remainingHealth) : EventArgs
{
    public int Damage => damage;
    public int MaxHp => maxHp;

    public int RemainingHealth => remainingHealth;
    public bool IsDead => wasDead;
}