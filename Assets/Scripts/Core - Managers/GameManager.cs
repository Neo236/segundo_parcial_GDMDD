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
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private SceneField mainMenuScene;
    //[SerializeField] private SpawnPointID mainMenuSpawnPoint;

    public SpawnPointID nextSpawnPointID;
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
        if (screenFader == null)
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
        Debug.Log($"Cambiando estado del juego a {CurrentGameState}");

        switch (CurrentGameState)
        {
            case GameState.MainMenu:
                playerObject.SetActive(false);
                if (UIManager.Instance != null && UIManager.Instance.PauseMenuScript != null)
                {
                    UIManager.Instance.PauseMenuScript.HideAllPausePanels();
                }

                if (inGameCanvas.transform.Find("HUD") != null)
                {
                    inGameCanvas.transform.Find("HUD").gameObject.SetActive(false);
                }
                Time.timeScale = 1f;
                break;
            
            case GameState.Gameplay:
                playerObject.SetActive(true);
                if (inGameCanvas.transform.Find("HUD") != null)
                {
                    inGameCanvas.transform.Find("HUD").gameObject.SetActive(true);
                }
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

    public void TransitionToScene(SceneField sceneToLoad, SpawnPointID spawnID, string sceneToUnload,
        AudioClip newMusic = null, bool stopMusic = false)
    {
        nextSpawnPointID = spawnID;
        StartCoroutine(TransitionCoroutine(sceneToLoad, sceneToUnload, newMusic, stopMusic));
    }

    private IEnumerator TransitionCoroutine(string sceneToLoad, string sceneToUnload, AudioClip newMusic, bool stopMusic)
    {
        SceneTransition.StartTransition();
        yield return StartCoroutine(screenFader.FadeOut());
        
        if (sceneToLoad == "MainMenu")
        {
            playerObject.SetActive(false);
        }
        
        if (inGameCanvas.transform.Find("HUD") != null)
        {
            inGameCanvas.transform.Find("HUD").gameObject.SetActive(false);
        }
        
        if (UIManager.Instance != null && UIManager.Instance.PauseMenuScript != null)
        {
            UIManager.Instance.PauseMenuScript.HideAllPausePanels();
        }
        
        if (!string.IsNullOrEmpty(sceneToUnload) && SceneManager.GetSceneByName(sceneToUnload).isLoaded)
        {
            Debug.Log($"Descargando escena: {sceneToUnload}");
            yield return SceneManager.UnloadSceneAsync(sceneToUnload);
        }
        
        if (stopMusic)
        {
            AudioManager.Instance.StopMusic();
        }
        else if (newMusic != null)
        {
            AudioManager.Instance.PlayMusic(newMusic);
        }

        Debug.Log($"Cargando escena: {sceneToLoad}");
        yield return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        _currentLoadedScene = sceneToLoad;
        
        //Moví la lógica de PlayerSpawn a acá para poder eliminarlo
        if (sceneToLoad != "MainMenu")
        {
            playerObject.SetActive(true);
        }

        if (nextSpawnPointID != null)
        {
            Debug.Log($"Buscando punto de spawn ID: {nextSpawnPointID.name}");
            SpawnPointIdentifier[] spawnPoints = FindObjectsOfType<SpawnPointIdentifier>();
            bool foundSpawn = false;
            
            foreach (SpawnPointIdentifier spawnPoint in spawnPoints)
            {
                if (spawnPoint.spawnPointID == GameManager.Instance.nextSpawnPointID)
                {
                    playerObject.transform.position = spawnPoint.transform.position;
                    Debug.Log($"Jugador movido a {spawnPoint.spawnPointID.name}");
                    foundSpawn = true;
                    break;
                }
            }

            if (!foundSpawn)
            {
                Debug.Log($"No se encontró punto de spawn ID: {GameManager.Instance.nextSpawnPointID.name}");
            }
        }

        if (MapRoomManager.Instance != null)
        {
            MapRoomManager.Instance.RevealRoom(sceneToLoad);
        }

        SetGameState(sceneToLoad != "MainMenu" ? GameState.Gameplay : GameState.MainMenu);

        yield return StartCoroutine(screenFader.FadeIn());
        SceneTransition.EndTransition();
    }

    public void GoToMainMenu()
    {
        string currentScene = _currentLoadedScene;
        TransitionToScene(
            mainMenuScene, 
            null, 
            currentScene, 
            null, 
            true
            );
    }
}
