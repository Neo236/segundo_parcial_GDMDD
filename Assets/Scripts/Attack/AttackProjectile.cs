using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    //public AttackData attackData;
    public float speed = 100f;

    Rigidbody2D rb;
    public float lifetime = 3f;

    private Vector3 moveDirection;

    private AttackData attackData;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        Destroy(gameObject, lifetime);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TO DO: Implementar lógica de colisión con enemigos o el entorno
        if (collision.CompareTag("Enemy"))
        {
            // Aquí podrías aplicar daño al enemigo
            Debug.Log("Hit an enemy!");
            EnemyClass enemy = collision.GetComponent<EnemyClass>();
            if (enemy != null && attackData != null)
            {
                enemy.TakeDamage(attackData); // Aplica el daño al enemigo
            }
            Destroy(gameObject); // Destruye el proyectil al colisionar
        }
        else if (collision.CompareTag("Environment"))
        {
            // Aquí podrías implementar lógica para colisiones con el entorno
            Debug.Log("Hit the environment!");
            Destroy(gameObject); // Destruye el proyectil al colisionar
        }

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
