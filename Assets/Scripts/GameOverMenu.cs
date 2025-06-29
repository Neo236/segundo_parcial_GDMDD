using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverMenu : MonoBehaviour
{
    [Header("New Game Settings")]
    [Tooltip("La configuración de la zona donde el jugador reaparecerá al reintentar.")]
    [SerializeField] private ZoneConfiguration startingZoneConfig;
    [SerializeField] private SpawnPointID spawnPointID;
    [SerializeField] private SceneField firstScene;
    
    [Header("Audio")]
    [Tooltip("La música que sonará en la pantalla de Game Over.")]
    [SerializeField] private AudioClip gameOverMusic;
    [Tooltip("La música que sonará al empezar el nuevo intento.")]
    [SerializeField] private AudioClip firstLevelMusic;
    
    [SerializeField] private GameObject retryButton;
    
    private void Start()
    {
        if (AudioManager.Instance != null && gameOverMusic != null)
        {
            AudioManager.Instance.PlayMusic(gameOverMusic);
        }
        
        // ✅ NUEVO: Usar UIManager centralizado
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetSelectedButton(retryButton);
        }

        //if (GameManager.Instance != null)
        //{
        //    _playerObject = GameManager.Instance.playerObject;
        //    _playerHealth = _playerObject.GetComponent<PlayerHealth>();
        //}
        
    }

    /// <summary>
    /// Esta función se debe conectar al botón "Reintentar" en el Inspector.
    /// Reinicia el progreso y comienza una nueva partida desde la zona de inicio.
    /// </summary>
    public void Retry()
    {
        if (startingZoneConfig == null)
        {
            Debug.LogError("¡No se ha asignado una ZoneConfiguration de inicio en el GameOverMenu! No se puede reintentar.");
            return;
        }

        Debug.Log("Reintentando... volviendo a la zona de inicio.");
        
        GameManager.Instance.ResetPlayerStateForNewGame();
        
        if (MapRoomManager.Instance != null) MapRoomManager.Instance.ResetAllZones();
        if (GlobalZoneSectorDetector.Instance != null) GlobalZoneSectorDetector.Instance.ResetAllZones();
        
        //PlayerHealth.ResetLife();

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
        Debug.Log("Volviendo al menú principal desde Game Over...");
        GameManager.Instance.GoToMainMenu();
    }
}