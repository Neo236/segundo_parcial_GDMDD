using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject backButton; // Reference to the first button to be selected
    
    //private MovementInput _movementInputScript;
    //private AttackInput _attackInputScript;
    private GameObject _player;
    
    //[SerializeField] private SettingsMenu settingsMenuScript;

    private void Awake()
    {
        _player = GameManager.Instance.playerObject;
        
        //_movementInputScript = _player.GetComponent<MovementInput>();
        //_attackInputScript = _player.GetComponent<AttackInput>();

        // Ensure both menus start inactive
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

    //private void Start()
    //{
    //    //MenuInput.OnPauseButtonPressed += HandlePauseInput;
    //}

    private void OnDestroy()
    {
        //MenuInput.OnPauseButtonPressed -= HandlePauseInput;
        
        if (UIManager.Instance != null && UIManager.Instance.SettingsMenuScript != null)
        {
            UIManager.Instance.SettingsMenuScript.OnBackAction -= ReturnFromSettings;           
        }
    }

   //public void HandlePauseInput()
   //{
   //    if (GameManager.Instance.CurrentGameState != GameState.Gameplay)
   //    {
   //        return;
   //    }
   //    
   //    // Don't handle pause input if a settings menu is active
   //    if (settingsMenuPanel.activeSelf)
   //        return;

   //    if (!_gameIsPaused)
   //    {
   //        Pause();
   //    }
   //    else
   //    {
   //        Resume();
   //    }
   //}

    private void DisableGameplayInputs()
    {
        if (_player != null)
        {
            _player.GetComponent<MovementInput>().enabled = false;
            _player.GetComponent<AttackInput>().enabled = false;
            // Add any other input scripts you want to disable here
        }
    }

    private void EnableGameplayInputs()
    {
        if (_player != null)
        {
            _player.GetComponent<MovementInput>().enabled = true;
            _player.GetComponent<AttackInput>().enabled = true;
            // Add any other input scripts you want to enable here
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        //Time.timeScale = 0f;
        //_gameIsPaused = true;
        DisableGameplayInputs();
        EventSystem.current.SetSelectedGameObject(resumeButton);
        GameManager.Instance.SetGameState(GameState.Paused);
    }

    public void Resume()
    {
        HideAllPausePanels();
        //Time.timeScale = 1f;
        //_gameIsPaused = false;
        EnableGameplayInputs();
        EventSystem.current.SetSelectedGameObject(null);
        GameManager.Instance.SetGameState(GameState.Gameplay);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Loading main menu...");
        //SceneManager.LoadScene("MainMenu");
        //Time.timeScale = 1f;
        //_gameIsPaused = false;
        
        GameManager.Instance.GoToMainMenu();
    }
    
    public void OpenInGameSettingsMenu()
    {
        settingsMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        UIManager.Instance.SettingsMenuScript.SetupInGameSliders();
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void ReturnFromSettings()
    {
        pauseMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void HideAllPausePanels()
    {
        pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
    }
}