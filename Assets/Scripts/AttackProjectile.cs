using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    public AttackData attackData;
    public float speed = 10f;
    public float lifetime = 5f; 

    private Vector3 moveDirection;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
        Destroy(gameObject, lifetime); 
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TO DO: Implementar lógica de colisión con enemigos o el entorno
        
    }
}
