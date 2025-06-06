using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject _mainMenuUI;
    private GameObject _optionsMenuUI;

    private void Awake()
    {
        _mainMenuUI = GameObject.FindWithTag("MainMenu");
        _optionsMenuUI = GameObject.FindWithTag("OptionsMenu");
        
        _mainMenuUI.SetActive(true);
        _optionsMenuUI.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("Starting Game...");
        //SceneManager.LoadScene("TutorialLevel");
    }

    public void OpenOptionsMenu()
    {
        _optionsMenuUI.SetActive(true);
        _mainMenuUI.SetActive(false);
    }

    public void CloseOptionsMenu()
    {
        _optionsMenuUI.SetActive(false);
        _mainMenuUI.SetActive(true);
    }
}
