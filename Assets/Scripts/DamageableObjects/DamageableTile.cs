using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DamageableTile : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool isInvulnerableToNonElemental = false;
    
    private float currentHealth;
    private bool isDead;
    private Tilemap tilemap;
    private TilemapCollider2D tilemapCollider;
    private CompositeCollider2D compositeCollider;
    private Rigidbody2D rb2d;

    public bool IsDead => isDead;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        SetupComponents();
        currentHealth = maxHealth;
    }

    private void SetupComponents()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
        compositeCollider = GetComponent<CompositeCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();

        if (tilemap == null || tilemapCollider == null || compositeCollider == null || rb2d == null)
        {
            Debug.LogError("DamageableTile: Missing required components!");
            enabled = false;
            return;
        }

        // Setup proper configuration for composite collider
        rb2d.bodyType = RigidbodyType2D.Static;
        tilemapCollider.usedByComposite = true;
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
    }

    public void TakeDamage(float damage, ElementType damageType = ElementType.None)
    {
        if (isDead) return;
        
        if (isInvulnerableToNonElemental && damageType == ElementType.None)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        // Update visual feedback for the entire tilemap
        Color currentColor = Color.white;
        float healthPercentage = currentHealth / maxHealth;
        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, healthPercentage);
        
        // Apply color to all tiles
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tilemap.SetTileFlags(pos, TileFlags.None);
                tilemap.SetColor(pos, newColor);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Clear all tiles
        tilemap.ClearAllTiles();
        
        // Destroy the game object
        Destroy(gameObject);
    }
}