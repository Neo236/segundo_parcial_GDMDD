using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    public AttackData attackData;
    public float speed = 100f;

    Rigidbody2D rb;
    public float lifetime = 5f; 

    private Vector3 moveDirection;



     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector3 direction)
    {

        moveDirection = direction.normalized;
        rb.linearVelocity = moveDirection * speed;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        
        Debug.Log("Projectile moving. Position: " + transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TO DO: Implementar lógica de colisión con enemigos o el entorno
        
    }
}
