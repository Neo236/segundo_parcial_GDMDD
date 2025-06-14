using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 10;

    private Transform target;

    public void SetTarget(Transform player)
    {
        target = player;
        Destroy(gameObject, lifetime); // Destruye el proyectil después de cierto tiempo
    }

    void Update()
    {
        if (target == null) return;

        // Mover el proyectil hacia el jugador
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Aquí puedes aplicar daño al jugador si tiene un script de salud
            // Ejemplo:
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            Debug.Log("Player hit by enemy attack!");
      
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Aplica daño al jugador
            }

            Destroy(gameObject); // Destruye el proyectil al impactar
        }
    }
}
