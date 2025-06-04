using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    //public AttackData attackData;
    public float speed = 100f;

    Rigidbody2D rb;
    public float lifetime = 3f; 

    private Vector3 moveDirection;

    private AttackData attackData;



     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector3 direction, AttackData attackData)
    {
        this.attackData = attackData;    
        moveDirection = direction.normalized;
        rb.linearVelocity = moveDirection * speed;
        Destroy(gameObject, lifetime);
    }

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TO DO: Implementar lógica de colisión con enemigos o el entorno
        
    }
}
