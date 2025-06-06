using UnityEngine;

public class EnemyDamageable : DamageableEntity
{
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private int scoreValue = 100;
    
    protected override void HandleDeath()
    {
        // Play death sound
        if (deathSound != null)
        {
            AudioManager.Instance.PlaySfx(deathSound);
        }
        
        // Additional enemy-specific death handling
        // For example: drop items, give score, play death animation, etc.
        Debug.Log($"Enemy died! Score: {scoreValue}");
        
        // Optionally destroy the game object
        Destroy(gameObject, 1f); // Destroy after 1 second (time for death animation)
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