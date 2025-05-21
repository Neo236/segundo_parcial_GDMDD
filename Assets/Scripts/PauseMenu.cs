using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused;
    
    private GameObject _pauseMenuUI;

    private void Awake()
    {
        _pauseMenuUI = GameObject.FindWithTag("PauseMenu");
    }

    public void ReadPauseInput(InputAction.CallbackContext context)
    {
        if (context.performed && GameIsPaused == false)
        {
            Pause();
        }
        else if (context.performed && GameIsPaused == true)
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
        SceneManager.LoadScene("MainMenu");
    }
    
    public void OpenInGameOptionsMenu()
}

