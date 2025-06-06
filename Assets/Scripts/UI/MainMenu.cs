using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    [Header("Scenes & Spawn Points")]
    [SerializeField] private string firstLevelSceneName = "Room 1";
    [SerializeField] private string firstLevelSpawnPoint = "SpawnPoint_FromStart";

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    
    //[Header("Component References")]
    //[SerializeField] private SettingsMenu settingsMenuScript;

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

    private void OnDestroy()
    {
        if (UIManager.Instance != null && UIManager.Instance.SettingsMenuScript != null)
        {
            UIManager.Instance.SettingsMenuScript.OnBackAction -= CloseSettingsMenu;
        }
    }

    public void StartGame()
    {
        Debug.Log($"Iniciando juego... cargando {firstLevelSceneName}");
        
        string sceneToUnload = gameObject.scene.name;

        GameManager.Instance.TransitionToScene(firstLevelSceneName, firstLevelSpawnPoint, sceneToUnload);
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
