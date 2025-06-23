using UnityEngine;
using UnityEngine.EventSystems;

public class EndSceneMenu : MonoBehaviour
{
    [Header("New Game Settings")]
    [Tooltip("La configuración de la zona donde el jugador reaparecerá al reintentar.")]
    [SerializeField] private ZoneConfiguration startingZoneConfig;
    [SerializeField] private SpawnPointID spawnPointID;
    [SerializeField] private SceneField firstScene;
    
    [Header("Audio")]
    [Tooltip("La música que sonará durante la escena final/créditos.")]
    [SerializeField] private AudioClip endSceneMusic;
    [Tooltip("La música que sonará al empezar el nuevo intento.")]
    [SerializeField] private AudioClip firstLevelMusic;
    
    [SerializeField] private GameObject retryButton;
    
    private void Start()
    {
        if (AudioManager.Instance != null && endSceneMusic != null)
        {
            AudioManager.Instance.PlayMusic(endSceneMusic);
        }
        
        // ✅ NUEVO: Usar UIManager centralizado
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetSelectedButton(retryButton);
        }
    }

    public void Retry()
    {
        if (startingZoneConfig == null)
        {
            Debug.LogError("¡No se ha asignado una ZoneConfiguration de inicio en el GameOverMenu! No se puede reintentar.");
            return;
        }

        Debug.Log("Reintentando... volviendo a la zona de inicio.");
        
        if (MapRoomManager.Instance != null) MapRoomManager.Instance.ResetAllZones();
        if (GlobalZoneSectorDetector.Instance != null) GlobalZoneSectorDetector.Instance.ResetAllZones();

        string sceneToUnload = gameObject.scene.name;
        GameManager.Instance.TransitionToScene(
            firstScene, 
            spawnPointID,
            sceneToUnload, 
            firstLevelMusic, 
            false
        );
    }
    
    /// <summary>
    /// Esta función se debe conectar al botón "Volver al Menú" en el Inspector.
    /// Llama a la función centralizada del GameManager.
    /// </summary>
    public void GoToMainMenu()
    {
        Debug.Log("Volviendo al menú principal desde la escena final...");
        GameManager.Instance.GoToMainMenu();
    }
}