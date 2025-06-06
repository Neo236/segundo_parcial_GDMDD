using UnityEngine;

public class PlayerDamageable : DamageableEntity
{
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    
    protected override void HandleDeath()
    {
        // Play death sound
        if (deathSound != null)
        {
            AudioManager.Instance.PlaySfx(deathSound);
        }
        
        // Additional player-specific death handling
        // For example: trigger game over, play death animation, etc.
        Debug.Log("Player died!");
    }

    public override void TakeDamage(float damage, ElementType damageType = ElementType.None)
    {
        base.TakeDamage(damage, damageType);

        // Play hurt sound
        if (!isDead && hurtSound != null)
        {
            AudioManager.Instance.PlaySfx(hurtSound);
        }
    }
}