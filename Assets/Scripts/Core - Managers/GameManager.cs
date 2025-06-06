using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Gameplay, Paused}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Object References")]
    public GameObject playerObject;
    [SerializeField] private GameObject inGameCanvas;
    private ScreenFader _screenFader;

    public string nextSpawnPointName;
    private string _currentLoadedScene;
    
    public GameState CurrentGameState { get; private set; }

    public void TogglePause()
    {
        if (CurrentGameState != GameState.Gameplay && CurrentGameState != GameState.Paused) return;
        if (CurrentGameState == GameState.Gameplay)
        {
            UIManager.Instance.PauseMenuScript.Pause();
        }
        else if (CurrentGameState == GameState.Paused)
        {
            UIManager.Instance.PauseMenuScript.Resume();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        _screenFader = inGameCanvas.GetComponentInChildren<ScreenFader>(true);
        if (_screenFader == null)
        {
            Debug.LogError("¡No se encontró el componente ScreenFader en el InGameCanvas asignado! Las transiciones no tendrán fundido a negro.");
        }
    }

    private void Start()
    {
        string mainMenuSceneName = "MainMenu";
        _currentLoadedScene = mainMenuSceneName;
        SceneManager.LoadSceneAsync(_currentLoadedScene, LoadSceneMode.Additive);
        
        SetGameState(GameState.MainMenu);
    }

    public void SetGameState(GameState newState)
    {
        CurrentGameState = newState;

        switch (CurrentGameState)
        {
            case GameState.MainMenu:
                playerObject.SetActive(false);
                inGameCanvas.SetActive(false);
                Time.timeScale = 1f;
                break;
            
            case GameState.Gameplay:
                playerObject.SetActive(true);
                inGameCanvas.SetActive(true);
                if (UIManager.Instance != null && UIManager.Instance.PauseMenuScript != null)
                {
                    UIManager.Instance.PauseMenuScript.HideAllPausePanels();
                }
                Time.timeScale = 1f;
                InitializeHUD();
                break;
            
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }

    private void InitializeHUD()
    {
        // Buscamos todos los componentes del HUD y los inicializamos uno por uno.
        inGameCanvas.GetComponentInChildren<HUD_Scripts.HealthBar>()?.Initialize();
        //inGameCanvas.GetComponentInChildren<HUD_Scripts.InkBar>()?.Initialize();
        //inGameCanvas.GetComponentInChildren<HUD_Scripts.HealthText>()?.Initialize();
        //inGameCanvas.GetComponentInChildren<HUD_Scripts.InkText>()?.Initialize();
        //inGameCanvas.GetComponentInChildren<AttackIcon>()?.Initialize();
        Debug.Log("HUD Inicializado.");
    }

    public void TransitionToScene(string sceneToLoad, string spawnPointName, string sceneToUnload)
    {
        nextSpawnPointName = spawnPointName;
        StartCoroutine(TransitionCoroutine(sceneToLoad, sceneToUnload));
    }

    private IEnumerator TransitionCoroutine(string sceneToLoad, string sceneToUnload)
    {
        SceneTransition.StartTransition();

        yield return StartCoroutine(_screenFader.FadeOut());

        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            Debug.Log($"Descargando escena: {sceneToUnload}");
            yield return SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        Debug.Log($"Cargando escena: {sceneToLoad}");
        yield return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        
        _currentLoadedScene = sceneToLoad;
        
        if (sceneToLoad != "MainMenu")
        {
            SetGameState(GameState.Gameplay);
        }
        else
        {
            SetGameState(GameState.MainMenu);
        }

        yield return StartCoroutine(_screenFader.FadeIn());
        SceneTransition.EndTransition();
    }

    public void GoToMainMenu()
    {
        SetGameState(GameState.MainMenu);
        TransitionToScene("MainMenu", "", _currentLoadedScene);
    }
}
