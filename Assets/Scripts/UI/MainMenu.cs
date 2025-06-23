using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    [Header("Scenes & Spawn Points")]
    [SerializeField] private SceneField firstLevelScene;
    [SerializeField] private SpawnPointID firstLevelSpawnPoint;
    [SerializeField] private ZoneConfiguration startingZoneConfig;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    
    [Header("Button Selection")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject settingsBackButton;
    
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
        
        // ✅ NUEVO: Usar UIManager centralizado para selección
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetSelectedButton(startButton);
        }
    }

    private void OnDestroy()
    {
        if (UIManager.Instance != null && UIManager.Instance.SettingsMenuScript != null)
        {
            UIManager.Instance.SettingsMenuScript.OnBackAction -= CloseSettingsMenu;
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
        
        if (GlobalZoneSectorDetector.Instance != null)
        {
            GlobalZoneSectorDetector.Instance.ResetAllZones();
        }
        
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
        
        // ✅ DEBUG: Verificar antes de activar paneles
        Debug.Log($"DEBUG ANTES - settingsBackButton: {(settingsBackButton != null ? settingsBackButton.name : "NULL")}");
    
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    
        // ✅ DEBUG: Verificar después de activar paneles
        Debug.Log($"DEBUG DESPUÉS - settingsBackButton: {(settingsBackButton != null ? settingsBackButton.name : "NULL")}");
    
        // ✅ CORREGIDO: Usar UIManager directamente
        if (UIManager.Instance != null && settingsBackButton != null)
        {
            UIManager.Instance.SetSelectedButton(settingsBackButton);
        }
        else
        {
            Debug.LogWarning($"MainMenu: No se puede seleccionar botón. UIManager: {(UIManager.Instance != null ? "OK" : "NULL")}, settingsBackButton: {(settingsBackButton != null ? "OK" : "NULL")}");
        }
        
        // ✅ DEBUG COMPLETO
        if (settingsBackButton != null)
        {
            Debug.Log($"DEBUG - settingsBackButton: {settingsBackButton.name}");
            Debug.Log($"DEBUG - settingsBackButton.activeInHierarchy: {settingsBackButton.activeInHierarchy}");
            Debug.Log($"DEBUG - settingsBackButton.activeSelf: {settingsBackButton.activeSelf}");
        }
        else
        {
            Debug.LogError("DEBUG - settingsBackButton es NULL!");
        }
        
        // ✅ CORREGIDO: Usar UIManager directamente
        if (UIManager.Instance != null && settingsBackButton != null)
        {
            UIManager.Instance.SetSelectedButton(settingsBackButton);
        }
        else
        {
            Debug.LogWarning($"MainMenu: No se puede seleccionar botón. UIManager: {(UIManager.Instance != null ? "OK" : "NULL")}, settingsBackButton: {(settingsBackButton != null ? "OK" : "NULL")}");
        }
    }

    public void CloseSettingsMenu()
    {
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
        // ✅ CORREGIDO: Usar UIManager directamente
        if (UIManager.Instance != null && startButton != null)
        {
            UIManager.Instance.SetSelectedButton(startButton);
        }
        else
        {
            Debug.LogWarning($"MainMenu: No se puede seleccionar botón. UIManager: {(UIManager.Instance != null ? "OK" : "NULL")}, startButton: {(startButton != null ? "OK" : "NULL")}");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}