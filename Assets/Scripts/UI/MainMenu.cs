using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    [Header("Scenes & Spawn Points")]
    [SerializeField] private SceneField firstLevelScene;
    [SerializeField] private SpawnPointID firstLevelSpawnPoint;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    
    //[Header("Component References")]
    //[SerializeField] private SettingsMenu settingsMenuScript;
    
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
        Debug.Log($"Iniciando juego... cargando {firstLevelScene.SceneName}...");
        string sceneToUnload = gameObject.scene.name;

        GameManager.Instance.TransitionToScene(firstLevelScene, 
            firstLevelSpawnPoint, sceneToUnload, firstLevelMusic, stopMusicOnLoad);
        
    }

    public void OpenSettingsMenu()
    {
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseSettingsMenu()
    {
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
