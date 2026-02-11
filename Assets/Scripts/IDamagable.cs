public interface IDamagable
{
    public int Health { get; }
    public int MaxHealth { get; }

    public void TakeDamage(float damage);
    public void Heal(int amount);
    public void Die();
}
