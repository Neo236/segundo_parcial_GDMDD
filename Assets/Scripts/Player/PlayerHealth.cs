using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event Action OnTakeDamage;
    
    [SerializeField]private AudioClip hurtSound;
    [SerializeField]private AudioClip deathSound;

    private float _currentHealth = 1000f;
    private float _maxHealth = 100f;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    public void TakeDamage(float damage)
    {
        AudioManager.Instance.PlaySfx(hurtSound);
        _currentHealth = CurrentHealth - damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
        OnTakeDamage?.Invoke();
    }

    public void Heal(float health)
    {
        _currentHealth = Math.Clamp(CurrentHealth + health, 0f, _maxHealth);
    }

    public void Die()
    {
        AudioManager.Instance.PlaySfx(deathSound);
    } 
}
