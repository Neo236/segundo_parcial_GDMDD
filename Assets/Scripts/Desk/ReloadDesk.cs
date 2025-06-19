using UnityEngine;

public class ReloadDesk : MonoBehaviour
{
    public bool playerIsOnTop = false;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Interact()
    {
        // Lógica que quieras ejecutar
        Debug.Log("El jugador interactuó con el objeto");
        // Ejemplo: abrir un menú, activar una animación, etc.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered by: {other.name}");
        if (other.CompareTag("Player"))
        {
            playerIsOnTop = true;
            player = other.gameObject;
            player.GetComponent<PlayerInk>().puedoRecargar = true;
            
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOnTop = false;
           player.GetComponent<PlayerInk>().puedoRecargar = false;
            player = null;
        }
    }
}
