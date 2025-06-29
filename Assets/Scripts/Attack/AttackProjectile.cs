using Unity.VisualScripting;
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

    private GameObject spriteHolder; // Reference to the GameObject that holds the sprite
    private CircleCollider2D circleCollider;

    private EnemyClass enemy; // Reference to the enemy that fired this projectile

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteHolder = transform.Find("SpriteHolder")?.gameObject;
        if(spriteHolder != null)
        {
            Debug.Log("SpriteHolder found in the scene.");
            spriteRenderer = spriteHolder.GetComponent<SpriteRenderer>();
        }
       
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
       
        moveDirection = new Vector2(direction.x, direction.y).normalized;
       
        rb.linearVelocity = moveDirection * speed;
        
         // Cambia el sprite aquí
        if (spriteRenderer != null && attackData != null && attackData.attackIcon != null)
        {
            spriteRenderer.sprite = attackData.attackIcon;
        }

        ChangeColorBasedOnElementType();
        // Set the projectile's layer to avoid friendly fire
      
        
        Destroy(gameObject, lifetime);
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackData == null) return;

        if (collision.gameObject.tag == "Enemy")
        {
            enemy = collision.gameObject.GetComponent<EnemyClass>();


            if (enemy == null)
            {
                return; // Ignore if the enemy is already dead
            }
            else
            {
                Debug.Log($"Enemy not found or already dead: {collision.gameObject.name}");
                enemy.TakeDamage(attackData);
                Destroy(gameObject); // Destroy the projectile after hitting the enemy
            }
            return; // Ignore self-collision
        }

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