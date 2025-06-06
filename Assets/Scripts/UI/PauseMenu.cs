using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject backButton; // Reference to the first button to be selected
    
    private bool _gameIsPaused;
    private MovementInput _movementInputScript;
    private AttackInput _attackInputScript;
    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _movementInputScript = _player.GetComponent<MovementInput>();
        _attackInputScript = _player.GetComponent<AttackInput>();

        // Ensure both menus start inactive
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
    }

    private void Start()
    {
        MenuInput.OnPauseButtonPressed += HandlePauseInput;
    }

    private void OnDestroy()
    {
        MenuInput.OnPauseButtonPressed -= HandlePauseInput;
    }

    public void HandlePauseInput()
    {
        // Don't handle pause input if settings menu is active
        if (settingsMenuPanel.activeSelf)
            return;

        if (!_gameIsPaused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    private void DisableGameplayInputs()
    {
        if (_movementInputScript != null)
            _movementInputScript.enabled = false;
        if (_attackInputScript != null)
            _attackInputScript.enabled = false;
        // Add any other input scripts you want to disable here
    }

    private void EnableGameplayInputs()
    {
        if (_movementInputScript != null)
            _movementInputScript.enabled = true;
        if (_attackInputScript != null)
            _attackInputScript.enabled = true;
        // Add any other input scripts you want to enable here
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        _gameIsPaused = true;
        DisableGameplayInputs();
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        _gameIsPaused = false;
        EnableGameplayInputs();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Loading main menu...");
        //SceneManager.LoadScene("MainMenu");
    }
    
    public void OpenInGameSettingsMenu()
    {
        settingsMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void ReturnFromSettings()
    {
        pauseMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }
}