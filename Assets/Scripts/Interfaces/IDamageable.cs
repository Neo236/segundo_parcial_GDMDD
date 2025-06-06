public interface IDamageable
{
    void TakeDamage(float damage, ElementType damageType = ElementType.None);
    void Die();
    bool IsDead { get; }
    float CurrentHealth { get; }
    float MaxHealth { get; }
}