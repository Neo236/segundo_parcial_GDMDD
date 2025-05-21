using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused;
    
    private GameObject _pauseMenuUI;
    private GameObject _inGameOptionsUI;

    private void Awake()
    {
        _pauseMenuUI = GameObject.FindWithTag("PauseMenu");
        _inGameOptionsUI = GameObject.FindWithTag("InGameOptionsMenu");
        
        _pauseMenuUI.SetActive(false);
        _inGameOptionsUI.SetActive(false);
    }

    public void ReadPauseInput(InputAction.CallbackContext context)
    {
        if (context.performed && !GameIsPaused)
        {
            Pause();
        }
        else if (context.performed && GameIsPaused)
        {
            Resume();
        }
    }

    public void Pause()
    {
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void GoToMainMenu()
    {
        Debug.Log("Loading main menu...");
        //SceneManager.LoadScene("MainMenu");
    }
    
    public void OpenInGameOptionsMenu()
    {
        _inGameOptionsUI.SetActive(true);
        _pauseMenuUI.SetActive(false);
    }

    public void CloseInGameOptionsMenu()
    {
        _inGameOptionsUI.SetActive(false);
        _pauseMenuUI.SetActive(true);
    }
}

