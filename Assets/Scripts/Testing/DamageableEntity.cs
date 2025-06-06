using UnityEngine;
using System;

public abstract class DamageableEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected ElementType weakness = ElementType.None;
    [SerializeField] protected ElementType resistance = ElementType.None;
    
    protected float currentHealth;
    protected bool isDead;

    public static event Action<DamageableEntity> OnAnyEntityDeath;
    public event Action<float> OnHealthChanged;
    public event Action<ElementType> OnDamageTaken;
    public event Action OnDeath;

    public bool IsDead => isDead;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage, ElementType damageType = ElementType.None)
    {
        if (isDead) return;

        float finalDamage = CalculateDamage(damage, damageType);
        currentHealth = Mathf.Clamp(currentHealth - finalDamage, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke(damageType);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual float CalculateDamage(float baseDamage, ElementType damageType)
    {
        float damageMultiplier = 1f;

        if (damageType == weakness)
        {
            damageMultiplier = 2f; // Double damage against weakness
        }
        else if (damageType == resistance)
        {
            damageMultiplier = 0.5f; // Half damage against resistance
        }

        return baseDamage * damageMultiplier;
    }

    public virtual void Die()
    {
        if (isDead) return;
        
        isDead = true;
        OnDeath?.Invoke();
        OnAnyEntityDeath?.Invoke(this);
        
        HandleDeath();
    }

    protected abstract void HandleDeath();
}