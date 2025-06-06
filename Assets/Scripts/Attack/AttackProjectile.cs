using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    public float speed = 100f;
    public float lifetime = 3f;
    [SerializeField] private LayerMask targetLayers; // Add this to specify what the projectile can hit

    private Rigidbody2D rb;
    private Vector3 moveDirection;
    private AttackData attackData;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();

        // Configure the collider as trigger
        if (circleCollider != null)
        {
            circleCollider.isTrigger = true;
        }
    }

    public void Initialize(Vector3 direction, AttackData attackData)
    {
        this.attackData = attackData;
        moveDirection = direction.normalized;
        rb.linearVelocity = moveDirection * speed;
        
        if(spriteRenderer != null)
        {
            ChangeColorBasedOnElementType();
        }
        
        // Set the projectile's layer to avoid friendly fire
        gameObject.layer = LayerMask.NameToLayer("Damageable");
        
        Destroy(gameObject, lifetime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackData == null) return;

        // Check if the collision is with a valid target layer
        if (((1 << collision.gameObject.layer) & targetLayers) == 0)
        {
            return; // Exit if the collided object is not in our target layers
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(attackData.baseDamage, attackData.elementType);
        }

        Destroy(gameObject);
    }
    
    private void ChangeColorBasedOnElementType()
    {
        if (spriteRenderer != null && attackData != null)
        {
            switch (attackData.elementType)
            {
                case ElementType.Fire:
                    spriteRenderer.color = Color.red;
                    break;
                case ElementType.Water:
                    spriteRenderer.color = Color.cyan;
                    break;
                case ElementType.Electric:
                    spriteRenderer.color = Color.yellow;
                    break;
                default:
                    spriteRenderer.color = Color.white;
                    break;
            }
        }
    }
}