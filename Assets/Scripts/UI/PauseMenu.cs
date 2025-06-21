using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject settingsBackButton;
    
    private GameObject _player;

    private void Awake()
    {
        _player = GameManager.Instance.playerObject;
        
        HideAllPausePanels();

        if (UIManager.Instance != null && UIManager.Instance.SettingsMenuScript != null)
        {
            UIManager.Instance.SettingsMenuScript.OnBackAction += ReturnFromSettings;           
        }
        else
        {
            Debug.Log("PauseMenu no pudo suscribirse al SettingsMenu " +
                      "porque UIManager o SettingsMenuScript no fueron encontrados.");
        }
    }

    private void OnDestroy()
    {
        if (UIManager.Instance != null && UIManager.Instance.SettingsMenuScript != null)
        {
            UIManager.Instance.SettingsMenuScript.OnBackAction -= ReturnFromSettings;           
        }
    }

    private void DisableGameplayInputs()
    {
        if (_player != null)
        {
            _player.GetComponent<MovementInput>().enabled = false;
            _player.GetComponent<AttackInput>().enabled = false;
        }
    }

    private void EnableGameplayInputs()
    {
        if (_player != null)
        {
            _player.GetComponent<MovementInput>().enabled = true;
            _player.GetComponent<AttackInput>().enabled = true;
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        DisableGameplayInputs();
        
        // ✅ NUEVO: Usar UIManager centralizado
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetSelectedButton(resumeButton);
        }
        
        GameManager.Instance.SetGameState(GameState.Paused);
    }

    public void Resume()
    {
        HideAllPausePanels();
        EnableGameplayInputs();
        
        // ✅ NUEVO: Usar UIManager centralizado para deseleccionar
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetSelectedButton(null);
        }
        
        GameManager.Instance.SetGameState(GameState.Gameplay);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Loading main menu...");
        GameManager.Instance.GoToMainMenu();
    }
    
    public void OpenInGameSettingsMenu()
    {
        settingsMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        UIManager.Instance.SettingsMenuScript.SetupInGameSliders();
        
        // ✅ NUEVO: Usar UIManager centralizado
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetSelectedButton(settingsBackButton);
        }
    }

    public void ReturnFromSettings()
    {
        pauseMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);
        
        // ✅ NUEVO: Usar UIManager centralizado
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetSelectedButton(resumeButton);
        }
    }

    public void HideAllPausePanels()
    {
        pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
    }
}