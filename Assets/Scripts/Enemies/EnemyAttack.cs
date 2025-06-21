using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 10;

    private Vector3 direccion;

    public void SetTarget(Transform player)
    {
        // Guardamos una vez la dirección al jugador
        direccion = (player.position - transform.position).normalized;
        Destroy(gameObject, lifetime); // Destruye el proyectil luego de un tiempo
    }
   

    void Update()
    {
        // Usamos la dirección fija guardada
        transform.position += speed * Time.deltaTime * direccion;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // Destruye el proyectil al impactar
        }
    }
}