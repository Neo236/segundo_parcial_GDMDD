using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    [Header("Scenes & Spawn Points")]
    [SerializeField] private SceneField firstLevelScene;
    [SerializeField] private SpawnPointID firstLevelSpawnPoint;
    [SerializeField] private ZoneConfiguration startingZoneConfig; // AÑADIR para reset

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    
    [Header("Button Selection")] // ✅ NUEVO: Referencias a botones
    [SerializeField] private GameObject startButton;    // Botón para seleccionar en main menu
    [SerializeField] private GameObject backButton;     // Botón para seleccionar en settings
    
    [Header("AudioClips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip firstLevelMusic;
    [SerializeField] private AudioClip continueMusic;
    [SerializeField] private bool stopMusicOnLoad;

    private void Awake()
    {
        mainMenuPanel.SetActive(true);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);

        if (UIManager.Instance != null && UIManager.Instance.SettingsMenuScript != null)
        {
            UIManager.Instance.SettingsMenuScript.OnBackAction += CloseSettingsMenu;            
        }
        else
        {
            Debug.Log("MainMenu no pudo suscribirse al SettingsMenu " +
                      "porque UIManager o SettingsMenuScript no fueron encontrados.");
        }
    }

    private void Start()
    {
        if (AudioManager.Instance != null && menuMusic != null)
        { 
            AudioManager.Instance.PlayMusic(menuMusic);
        }
        
        // ✅ NUEVO: Seleccionar el botón Start al iniciar
        SelectStartButton();
    }

    private void OnDestroy()
    {
        if (UIManager.Instance != null && UIManager.Instance.SettingsMenuScript != null)
        {
            UIManager.Instance.SettingsMenuScript.OnBackAction -= CloseSettingsMenu;
        }
    }

    // ✅ NUEVO: Método para seleccionar el botón Start
    private void SelectStartButton()
    {
        if (startButton != null)
        {
            EventSystem.current.SetSelectedGameObject(startButton);
            Debug.Log("Botón Start seleccionado");
        }
        else
        {
            Debug.LogWarning("Start Button no está asignado en MainMenu");
        }
    }

    // ✅ NUEVO: Método para seleccionar el botón Back
    private void SelectBackButton()
    {
        if (backButton != null)
        {
            EventSystem.current.SetSelectedGameObject(backButton);
            Debug.Log("Botón Back seleccionado en Settings");
        }
        else
        {
            Debug.LogWarning("Back Button no está asignado en MainMenu");
        }
    }

    public void StartGame()
    {
        if (firstLevelScene == null || firstLevelSpawnPoint == null)
        {
            Debug.LogError("La escena o el punto de spawn de inicio no están asignados en el MainMenu!");
            return;
        }
        
        Debug.Log($"Iniciando juego nuevo... cargando {firstLevelScene.SceneName}...");
        
        // AGREGAR: Resetear detector global al iniciar nuevo juego
        if (GlobalZoneSectorDetector.Instance != null)
        {
            GlobalZoneSectorDetector.Instance.ResetAllZones();
        }
        
        // SOLO añadir esta línea para reset de juego nuevo:
        if (MapRoomManager.Instance != null && startingZoneConfig != null)
        {
            MapRoomManager.Instance.ResetAllZones();
        }
        
        string sceneToUnload = gameObject.scene.name;
        GameManager.Instance.TransitionToScene(firstLevelScene, 
            firstLevelSpawnPoint, sceneToUnload, firstLevelMusic, stopMusicOnLoad);
    }

    public void OpenSettingsMenu()
    {
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        
        // ✅ NUEVO: Seleccionar botón Back cuando se abre Settings
        SelectBackButton();
    }

    public void CloseSettingsMenu()
    {
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
        // ✅ NUEVO: Volver a seleccionar botón Start cuando se cierra Settings
        SelectStartButton();
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}