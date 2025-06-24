using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine; // ✅ CORRECTO para Cinemachine v3

public enum GameState { MainMenu, Gameplay, Paused, PlayerDead}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Object References")]
    public GameObject playerObject;
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private SceneField mainMenuScene;
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private SceneField gameOverScene;
    [SerializeField] private SceneField endScene;
    
    [Header("Camera System")]
    [SerializeField] private GameObject defaultVirtualCameraPrefab; // ✅ NUEVO!

    public SpawnPointID nextSpawnPointID;
    private string _currentLoadedScene;
    // ✅ Ya no necesitamos _activeVCam porque crearemos/destruiremos dinámicamente
    
    public GameState CurrentGameState { get; private set; }

    [SerializeField] private EnemyDistance[] enemies;
  

    private bool gameEnded = false;

    void Update()
    {
        if (gameEnded) return;

        bool allDead = true;
        foreach (EnemyDistance enemy in enemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            gameEnded = true;
            Debug.Log("All enemies defeated. Loading end scene...");
         
        }
    }

    public void TogglePause()
    {
        if (CurrentGameState != GameState.Gameplay && CurrentGameState != GameState.Paused ) return;
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
            Debug.LogError("¡No se encontró el componente ScreenFader en el InGameCanvas asignado! " +
                           "Las transiciones no tendrán fundido a negro.");
        }
    }

    private void Start()
    {
        if (MapRoomManager.Instance != null)
        {
            MapRoomManager.Instance.ResetAllZones();
        }
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

                if (mapCanvas != null)
                {
                    mapCanvas.SetActive(false);
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

                if (mapCanvas != null)
                {
                    mapCanvas.SetActive(true);
                }
                Time.timeScale = 1f;
                InitializeHUD();
                break;
            
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            
            case GameState.PlayerDead:
                // El jugador está muerto.
                // Los inputs se desactivarán, pero el tiempo sigue corriendo.
                Time.timeScale = 1f;
                // Opcional: Podrías querer ocultar el HUD aquí
                if (inGameCanvas.transform.Find("HUD") != null)
                {
                    inGameCanvas.transform.Find("HUD").gameObject.SetActive(false);
                }
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
        StartCoroutine(TransitionCoroutine(sceneToLoad.SceneName, sceneToUnload, newMusic, stopMusic));
    }

    private IEnumerator TransitionCoroutine(string sceneToLoad, string sceneToUnload, AudioClip newMusic, bool stopMusic)
    {
        SceneTransition.StartTransition();

        // 1. DESTRUIR CÁMARA VIRTUAL ANTERIOR
        var oldVCam = FindObjectOfType<CinemachineCamera>();
        if (oldVCam != null)
        {
            Destroy(oldVCam.gameObject);
            Debug.Log($"📷 Cámara anterior destruida: {oldVCam.name}");
        }

        yield return StartCoroutine(screenFader.FadeOut());
                
        bool isGameplayScene = (
            sceneToLoad != mainMenuScene.SceneName && 
            sceneToLoad != gameOverScene.SceneName && 
            sceneToLoad != endScene.SceneName
        );
        
        if (!isGameplayScene)
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
        
        yield return new WaitForEndOfFrame();

        // --- LÓGICA MEJORADA DE INSTANCIACIÓN ---
        if (sceneToLoad != "MainMenu")
        {
            var zoneConfig = GlobalZoneSectorDetector.Instance?.FindConfigForScene(sceneToLoad);
        
            if (zoneConfig != null)
            {
                // 1. DECIDIR QUÉ PREFAB USAR
                GameObject prefabToInstantiate = zoneConfig.virtualCameraPrefab != null 
                    ? zoneConfig.virtualCameraPrefab 
                    : defaultVirtualCameraPrefab;

                if (prefabToInstantiate == null)
                {
                    Debug.LogError("❌ No se encontró ningún prefab de cámara para instanciar.");
                    PerformFallbackSpawn();
                    yield break;
                }

                // 2. ASEGURAR QUE EL JUGADOR ESTÉ ACTIVO ANTES DE ASIGNAR
                if (!playerObject.activeInHierarchy)
                {
                    playerObject.SetActive(true);
                    Debug.Log("🎯 Jugador activado antes de configurar cámara");
                }

                // 3. MOVER AL JUGADOR PRIMERO
                if (nextSpawnPointID != null)
                {
                    var spawnIdentifier = FindSpawnPoint(nextSpawnPointID);
                    if (spawnIdentifier != null)
                    {
                        Vector3 spawnPosition = spawnIdentifier.transform.position;
                        playerObject.transform.position = spawnPosition;
                        Debug.Log($"🚀 Jugador movido PRIMERO a spawn: {spawnPosition}");
                    }
                }

                yield return new WaitForEndOfFrame(); // Asegurar que el jugador esté en posición

                // 4. INSTANCIAR LA CÁMARA
                GameObject vcamObject = Instantiate(prefabToInstantiate);
                vcamObject.name = $"VCam_{zoneConfig.zoneName}";
                CinemachineCamera newVCam = vcamObject.GetComponent<CinemachineCamera>();

                if (newVCam != null)
                {
                    Debug.Log($"📷 Nueva cámara instanciada: {newVCam.name}");

                    // 5. APLICAR SOBRESCRITURAS ANTES DE ASIGNAR TARGET
                    if (zoneConfig.overrideCameraSettings)
                    {
                        ApplyCameraOverrides(newVCam, zoneConfig);
                    }

                    // 6. ASIGNAR TARGET CON VERIFICACIONES
                    Debug.Log($"🎯 Asignando jugador {playerObject.name} (activo: {playerObject.activeInHierarchy}) como target...");
                
                    newVCam.Follow = playerObject.transform;
                    newVCam.LookAt = playerObject.transform;
                
                    // ✅ VERIFICAR QUE LA ASIGNACIÓN FUNCIONÓ
                    if (newVCam.Follow == playerObject.transform)
                    {
                        Debug.Log($"✅ Follow target asignado correctamente: {newVCam.Follow.name}");
                    }
                    else
                    {
                        Debug.LogError($"❌ ERROR: Follow target NO se asignó correctamente");
                    }

                    if (newVCam.LookAt == playerObject.transform)
                    {
                        Debug.Log($"✅ LookAt target asignado correctamente: {newVCam.LookAt.name}");
                    }
                    else
                    {
                        Debug.LogError($"❌ ERROR: LookAt target NO se asignó correctamente");
                    }

                    // 7. ASEGURAR PRIORIDAD DE CÁMARA
                    newVCam.Priority = 10; // Prioridad alta para que Cinemachine la detecte
                    Debug.Log($"📊 Prioridad de cámara establecida en: {newVCam.Priority}");

                    // 8. GENERAR Y ASIGNAR BOUNDS
                    if (CameraBoundsGenerator.Instance != null)
                    {
                        Debug.Log($"🚀 Generando bounds para {zoneConfig.zoneName}...");
                        CameraBoundsGenerator.Instance.GenerateAndAssignBounds(zoneConfig, newVCam);
                        yield return new WaitForEndOfFrame();
                    }

                    // 9. FORZAR ACTUALIZACIÓN DE CINEMACHINE
                    Debug.Log("📡 Forzando actualización de Cinemachine...");
                
                    // Informar del teletransporte si es necesario
                    if (nextSpawnPointID != null)
                    {
                        newVCam.OnTargetObjectWarped(playerObject.transform, Vector3.zero);
                        Debug.Log("📡 Cinemachine informado del teletransporte");
                    }

                    // Forzar a Cinemachine Brain a detectar la nueva cámara
                    var cinemachineBrain = FindObjectOfType<CinemachineBrain>();
                    if (cinemachineBrain != null)
                    {
                        cinemachineBrain.ManualUpdate();
                        Debug.Log($"🧠 Cinemachine Brain actualizado manualmente. Cámara activa: {cinemachineBrain.ActiveVirtualCamera?.Name ?? "NINGUNA"}");
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ No se encontró CinemachineBrain en la escena");
                    }

                    yield return new WaitForEndOfFrame();

                    // 10. VERIFICACIONES FINALES
                    Debug.Log("🔍 === VERIFICACIONES FINALES ===");
                    Debug.Log($"🎯 Jugador activo: {playerObject.activeInHierarchy}");
                    Debug.Log($"🎯 Posición del jugador: {playerObject.transform.position}");
                    Debug.Log($"📷 Cámara activa: {newVCam.gameObject.activeInHierarchy}");
                    Debug.Log($"📷 Follow target: {newVCam.Follow?.name ?? "NULL"}");
                    Debug.Log($"📷 LookAt target: {newVCam.LookAt?.name ?? "NULL"}");
                    Debug.Log($"📷 Prioridad: {newVCam.Priority}");

                    // 11. CONFIRMAR DETECCIÓN DE ZONA
                    GlobalZoneSectorDetector.Instance.ForcePlayerDetection(playerObject.transform);
                
                    Debug.Log("🎉 CONFIGURACIÓN DE CÁMARA COMPLETADA");
                }
                else
                {
                    Debug.LogError($"❌ El prefab {prefabToInstantiate.name} no tiene componente CinemachineCamera");
                    PerformFallbackSpawn();
                }
            }
            else
            {
                Debug.LogError($"❌ FATAL: No se encontró ZoneConfig para la escena {sceneToLoad}");
                PerformFallbackSpawn();
            }
        }
        
        SetGameState(isGameplayScene ? GameState.Gameplay : GameState.MainMenu);

        yield return StartCoroutine(screenFader.FadeIn());
        SceneTransition.EndTransition();
    }

    // ✅ CORREGIDO: Método para aplicar configuraciones personalizadas (compatible con Cinemachine v3)
    private void ApplyCameraOverrides(CinemachineCamera vcam, ZoneConfiguration zoneConfig)
    {
        try
        {
            // Cambiar tamaño ortográfico
            vcam.Lens.OrthographicSize = zoneConfig.cameraSize;
            Debug.Log($"📐 Tamaño de cámara overrideado a: {zoneConfig.cameraSize}");

            // ✅ CORREGIDO: Buscar y configurar Framing Transposer (nombres actualizados para v3)
            var transposer = vcam.GetComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                // ✅ En v3, las propiedades de damping están en estos nombres:
                transposer.m_XDamping = zoneConfig.cameraDamping;
                transposer.m_YDamping = zoneConfig.cameraDamping;
                
                // ✅ En v3, las propiedades de dead zone están en estos nombres:
                transposer.m_DeadZoneWidth = zoneConfig.deadZoneWidth;
                transposer.m_DeadZoneHeight = zoneConfig.deadZoneHeight;
                
                Debug.Log($"🎛️ Transposer configurado - Damping: {zoneConfig.cameraDamping}, Dead Zone: {zoneConfig.deadZoneWidth}x{zoneConfig.deadZoneHeight}");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró CinemachineFramingTransposer en la cámara para aplicar overrides");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"⚠️ Error aplicando overrides de cámara: {ex.Message}");
        }
    }

    public void GoToMainMenu()
    {
        // AGREGAR: Resetear detector global antes de ir al menú
        if (GlobalZoneSectorDetector.Instance != null)
        {
            GlobalZoneSectorDetector.Instance.ResetAllZones();
        }
        
        string currentScene = _currentLoadedScene;
        TransitionToScene(
            mainMenuScene, 
            null, 
            currentScene, 
            null, 
            true
            );
    }

    // AGREGAR: Método helper para encontrar spawn point
    private SpawnPointIdentifier FindSpawnPoint(SpawnPointID spawnID)
    {
        if (spawnID == null) return null;
        
        SpawnPointIdentifier[] allSpawnPoints = FindObjectsOfType<SpawnPointIdentifier>();
        foreach (var spawn in allSpawnPoints)
        {
            if (spawn.spawnPointID == spawnID)
            {
                return spawn;
            }
        }
        
        Debug.LogWarning($"⚠️ No se encontró SpawnPoint con ID: {spawnID.name}");
        return null;
    }

    // AGREGAR: Método de respaldo si algo falla
    private void PerformFallbackSpawn()
    {
        Debug.LogWarning("⚠️ Ejecutando spawn de respaldo...");
        
        // Intentar encontrar cualquier SpawnPointIdentifier en la escena
        SpawnPointIdentifier[] availableSpawns = FindObjectsOfType<SpawnPointIdentifier>();
        if (availableSpawns.Length > 0)
        {
            Vector3 fallbackPosition = availableSpawns[0].transform.position;
            playerObject.transform.position = fallbackPosition;
            playerObject.SetActive(true);
            Debug.Log($"🆘 Jugador posicionado en spawn de respaldo: {fallbackPosition}");
        }
        else
        {
            // Último recurso: posicionar en (0,0,0)
            playerObject.transform.position = Vector3.zero;
            playerObject.SetActive(true);
            Debug.Log("🆘 Jugador posicionado en origen (0,0,0)");
        }
    }
    public void TriggerGameOver()
    {
        // Nos aseguramos de que solo se pueda morir mientras se juega
        if (CurrentGameState == GameState.MainMenu) return;

        Debug.Log("GAME OVER - Iniciando transición...");
        string currentScene = _currentLoadedScene;
    
        // Reutilizamos la misma lógica de transición
        TransitionToScene(
            gameOverScene,      // <-- Usamos la nueva referencia
            null,               // Sin spawn point
            currentScene,
            null,               // Sin música nueva (la escena se encargará)
            true                // Detenemos la música del nivel
        );
    }

// Este método se llamará al derrotar al jefe final
    public void TriggerEndScene()
    {
        if (CurrentGameState != GameState.Gameplay) return;

        Debug.Log("VICTORIA - Cargando escena final...");
        string currentScene = _currentLoadedScene;

        TransitionToScene(
            endScene,           // <-- Usamos la nueva referencia
            null,
            currentScene,
            null,
            true
        );
    }
    
    public void ResetPlayerStateForNewGame()
    {
        if (playerObject == null) return;

        Debug.Log("Reiniciando estado del jugador para una nueva partida...");

        // 1. Resetear la salud
        PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ReviveAndResetHealth(); // Usaremos este nuevo método
        }

        // 2. Resetear la tinta
        PlayerInk playerInk = playerObject.GetComponent<PlayerInk>();
        if (playerInk != null)
        {
            playerInk.ResetInk(); // Necesitarás crear este método
        }
    }
}