using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    //public static event Action<bool> OnGamePauseStateChanged;
    private bool _gameIsPaused;
    
    private MovementInput _movementInputScript;
    private GameObject _pauseMenuUI;
    private GameObject _inGameSettingsUI;
    private GameObject _resumeButton;
    private GameObject _settingsBackButton;

    private void Awake()
    {
        _pauseMenuUI = GameObject.FindWithTag("PauseMenu");
        _inGameSettingsUI = GameObject.FindWithTag("InGameSettingsMenu");
        _resumeButton = _pauseMenuUI.transform.Find("ResumeButton").gameObject;
        _settingsBackButton = _inGameSettingsUI.transform.Find("BackButton").gameObject;
        
        _pauseMenuUI.SetActive(false);
        _inGameSettingsUI.SetActive(false);
        
        MenuInput.OnPauseButtonPressed += HandlePauseInput;
    }

    public void HandlePauseInput()
    {
        if (!_gameIsPaused)
        {
            Pause();
        }
        else if (_gameIsPaused)
        {
            Resume();
        }
    }

    public void Pause()
    {
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _gameIsPaused = true;
        EventSystem.current.SetSelectedGameObject(_resumeButton);
    }

    public void Resume()
    {
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _gameIsPaused = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Loading main menu...");
        //SceneManager.LoadScene("MainMenu");
    }
    
    public void OpenInGameSettingsMenu()
    {
        _inGameSettingsUI.SetActive(true);
        _pauseMenuUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_settingsBackButton);
    }

    public void CloseInGameSettingsMenu()
    {
        _inGameSettingsUI.SetActive(false);
        _pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_resumeButton);
    }
}

