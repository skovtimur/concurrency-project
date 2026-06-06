using ConcurrencyConsoleProj.Shared;

namespace ConcurrencyConsoleProj.EventHandlers;

public class Player : IUnit, IDamageable
{
    public Player(int health, int maxHp)
    {
        if (health > maxHp)
        {
            throw new ArgumentException("The current player's health is out of the max hp");
        }

        _health = health;
        _maxHp = maxHp;
    }

    public int Health => _health;
    public bool IsDead => _health <= 0;
    private int _health;
    private readonly int _maxHp;

    // IMPORTANT:
    // Event Handler is a delegate, nothing more, just a delegate that brings sender(where it called from) and some custom data (event args)
    public event EventHandler<PlayerDamagedEventArgs>? OnDamaged
    {
        add
        {
            Logger.Log("A new subscriber is joining!", ConsoleColor.DarkGray);
            _onDamagedDelegate += value; // 'value' is the incoming handler
        }
        remove
        {
            Logger.Log("A subscriber left.", ConsoleColor.DarkGray);
            _onDamagedDelegate -= value; // 'value' is the handler to remove
        }
    }

    // btw, an event is like a property, there's some field and its property, it's the same thing for delegates and events. Encapsulation 
    private EventHandler<PlayerDamagedEventArgs>? _onDamagedDelegate;

    public void TakeDamage(int damage)
    {
        _health = Math.Max(0, _health - damage);
        _onDamagedDelegate?.Invoke(this, new PlayerDamagedEventArgs(damage, IsDead, _maxHp, _health));
    }
}