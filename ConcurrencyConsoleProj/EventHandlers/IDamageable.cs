namespace ConcurrencyConsoleProj.EventHandlers;

public interface IDamageable
{
    public event EventHandler<PlayerDamagedEventArgs>? OnDamaged;
    public void TakeDamage(int damage);
}