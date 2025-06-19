// EndSceneMenu.cs
using UnityEngine;

public class EndSceneMenu : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("La música que sonará durante la escena final/créditos.")]
    [SerializeField] private AudioClip endSceneMusic;

    private void Start()
    {
        // Pone la música de la escena final.
        if (AudioManager.Instance != null && endSceneMusic != null)
        {
            AudioManager.Instance.PlayMusic(endSceneMusic);
        }
    }

    /// <summary>
    /// Esta función se debe conectar al botón "Volver al Menú" en el Inspector.
    /// Llama a la función centralizada del GameManager.
    /// </summary>
    public void GoToMainMenu()
    {
        Debug.Log("Volviendo al menú principal desde la escena final...");
        
        // Le decimos al GameManager que se encargue de todo.
        GameManager.Instance.GoToMainMenu();
    }
}