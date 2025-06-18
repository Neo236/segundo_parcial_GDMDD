// GlobalZoneSectorDetector.cs - Detector único persistente que funciona en todas las zonas
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GlobalZoneSectorDetector : MonoBehaviour
{
    public static GlobalZoneSectorDetector Instance { get; private set; }
    
    [Header("Zone Configuration")]
    [Tooltip("Todas las configuraciones de zonas del juego")]
    public ZoneConfiguration[] allZoneConfigs;
    
    [Header("Player Detection")]
    public string playerTag = "Player";
    
    [Header("Detection Settings")]
    public float detectionInterval = 0.1f;
    public bool autoFindPlayer = true;
    
    [Header("Debug")]
    public bool showDebugGizmos = true;
    public bool showDebugInfo = true;
    
    // Estado global del mapa
    private Dictionary<int, bool[,]> globalRevealedSectors = new Dictionary<int, bool[,]>();
    
    // Estado actual
    private Transform playerTransform;
    private ZoneConfiguration currentZoneConfig;
    private Vector2Int currentSectorGrid = new Vector2Int(-1, -1);
    private int currentSectorNumber = -1;
    private float lastDetectionTime = 0f;
    private bool isInitialized = false;
    
    private void Awake()
    {
        // Singleton persistente
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Suscribirse a cambios de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Inicializar arrays de sectores revelados para cada zona
        InitializeGlobalSectors();
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void InitializeGlobalSectors()
    {
        foreach (var config in allZoneConfigs)
        {
            if (config != null)
            {
                globalRevealedSectors[config.zoneNumber] = 
                    new bool[config.gridWidth, config.gridHeight];
            }
        }
        
        Debug.Log($"GlobalZoneSectorDetector: Inicializadas {globalRevealedSectors.Count} zonas");
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // CAMBIAR: Detectar tanto Single como Additive
        Debug.Log($"GlobalZoneSectorDetector: OnSceneLoaded llamado para '{scene.name}' (modo: {mode})");
        
        // Procesar cualquier modo de carga
        StartCoroutine(DelayedInitialize(scene.name));
    }
    
    private IEnumerator DelayedInitialize(string sceneName)
    {
        // Esperar un frame para que todo esté cargado
        yield return null;
        
        // DEBUG: Mostrar escena recibida
        Debug.Log($"GlobalZoneSectorDetector: DelayedInitialize para '{sceneName}'");
        
        // Ignorar ciertas escenas
        if (sceneName == "PersistentScene" || sceneName == "MainMenu")
        {
            Debug.Log($"GlobalZoneSectorDetector: Ignorando escena '{sceneName}'");
            isInitialized = false;
            currentZoneConfig = null;
            yield break;
        }
        
        InitializeForScene(sceneName);
    }
    
    private void InitializeForScene(string sceneName)
    {
        // DEBUG: Mostrar configuraciones disponibles
        Debug.Log($"GlobalZoneSectorDetector: Buscando configuración para '{sceneName}'");
        Debug.Log($"GlobalZoneSectorDetector: Configuraciones disponibles: {allZoneConfigs.Length}");
        
        foreach (var config in allZoneConfigs)
        {
            if (config != null)
            {
                string configSceneName = config.zoneScene != null ? config.zoneScene.SceneName : "NULL";
                Debug.Log($"GlobalZoneSectorDetector: Config {config.zoneName} -> escena: '{configSceneName}'");
            }
        }
        
        // Buscar configuración para esta escena
        currentZoneConfig = null;
        foreach (var config in allZoneConfigs)
        {
            if (config != null && config.zoneScene != null && 
                config.zoneScene.SceneName == sceneName)
            {
                currentZoneConfig = config;
                Debug.Log($"GlobalZoneSectorDetector: ¡ENCONTRADA configuración para '{sceneName}': {config.zoneName}");
                break;
            }
        }
        
        if (currentZoneConfig == null)
        {
            if (showDebugInfo) Debug.LogWarning($"GlobalZoneSectorDetector: No se encontró configuración para '{sceneName}'");
            isInitialized = false;
            return;
        }
        
        // Buscar jugador en la nueva escena
        FindPlayer();
        
        // Registrar zona en MapRoomManager
        if (MapRoomManager.Instance != null)
        {
            MapRoomManager.Instance.RegisterZone(currentZoneConfig);
            
            // Restaurar sectores ya revelados
            RestoreRevealedSectors();
        }
        
        // Reset tracking
        currentSectorGrid = new Vector2Int(-1, -1);
        currentSectorNumber = -1;
        isInitialized = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"GlobalZoneSectorDetector: Inicializado para {currentZoneConfig.zoneName} " +
                     $"({currentZoneConfig.gridWidth}x{currentZoneConfig.gridHeight} sectores)");
        }
    }
    
    private void FindPlayer()
    {
        if (autoFindPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            playerTransform = player != null ? player.transform : null;
            
            if (showDebugInfo)
            {
                Debug.Log($"GlobalZoneSectorDetector: Jugador {(playerTransform != null ? "encontrado" : "NO encontrado")}");
            }
        }
    }
    
    private void RestoreRevealedSectors()
    {
        if (!globalRevealedSectors.ContainsKey(currentZoneConfig.zoneNumber)) return;
        
        var revealedSectors = globalRevealedSectors[currentZoneConfig.zoneNumber];
        
        for (int x = 0; x < currentZoneConfig.gridWidth; x++)
        {
            for (int y = 0; y < currentZoneConfig.gridHeight; y++)
            {
                if (revealedSectors[x, y])
                {
                    int sectorNumber = currentZoneConfig.GridToSectorNumber(new Vector2Int(x, y));
                    MapRoomManager.Instance.RevealZoneSector(currentZoneConfig.zoneNumber, sectorNumber);
                }
            }
        }
        
        if (showDebugInfo)
        {
            int revealedCount = 0;
            foreach (bool revealed in revealedSectors)
                if (revealed) revealedCount++;
            
            Debug.Log($"GlobalZoneSectorDetector: Restaurados {revealedCount} sectores en {currentZoneConfig.zoneName}");
        }
    }
    
    private void Update()
    {
        if (!isInitialized || playerTransform == null || currentZoneConfig == null) return;
        
        if (Time.time - lastDetectionTime >= detectionInterval)
        {
            CheckPlayerSector();
            lastDetectionTime = Time.time;
        }
    }
    
    private void CheckPlayerSector()
    {
        Vector2 playerPos = playerTransform.position;
        Vector2Int newGridPos = currentZoneConfig.WorldToGridPosition(playerPos);
        
        // Verificar si está fuera de bounds
        if (newGridPos.x < 0 || newGridPos.y < 0)
        {
            if (showDebugInfo && currentSectorNumber != -1)
            {
                Debug.Log($"GlobalZoneSectorDetector: Jugador fuera de la zona de sectores en {currentZoneConfig.zoneName}");
                
                // AGREGAR: Notificar al MapRoomManager que salió de todos los sectores
                if (MapRoomManager.Instance != null && currentSectorNumber != -1)
                {
                    MapRoomManager.Instance.UpdatePlayerPosition(currentZoneConfig.zoneName, -1);
                }
                
                currentSectorGrid = new Vector2Int(-1, -1);
                currentSectorNumber = -1;
            }
            return;
        }
        
        // Solo procesar si cambió de sector
        if (newGridPos != currentSectorGrid)
        {
            currentSectorGrid = newGridPos;
            currentSectorNumber = currentZoneConfig.GridToSectorNumber(newGridPos);
            
            // Revelar sector si no estaba revelado
            var revealedSectors = globalRevealedSectors[currentZoneConfig.zoneNumber];
            if (!revealedSectors[newGridPos.x, newGridPos.y])
            {
                RevealSector(newGridPos.x, newGridPos.y);
            }
            
            // AGREGAR: Notificar al MapRoomManager la nueva posición
            if (MapRoomManager.Instance != null)
            {
                MapRoomManager.Instance.UpdatePlayerPosition(currentZoneConfig.zoneName, currentSectorNumber);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"GlobalZoneSectorDetector: Jugador en sector {currentSectorNumber} " +
                         $"(Grid: {newGridPos}) de {currentZoneConfig.zoneName}");
            }
        }
    }
    
    public void RevealSector(int gridX, int gridY)
    {
        if (!isInitialized || currentZoneConfig == null) return;
        
        if (gridX < 0 || gridX >= currentZoneConfig.gridWidth || 
            gridY < 0 || gridY >= currentZoneConfig.gridHeight) return;
        
        // Marcar como revelado globalmente
        globalRevealedSectors[currentZoneConfig.zoneNumber][gridX, gridY] = true;
        
        int sectorNumber = currentZoneConfig.GridToSectorNumber(new Vector2Int(gridX, gridY));
        
        // Notificar al MapRoomManager
        if (MapRoomManager.Instance != null)
        {
            MapRoomManager.Instance.RevealZoneSector(currentZoneConfig.zoneNumber, sectorNumber);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"GlobalZoneSectorDetector: Sector {sectorNumber} revelado en {currentZoneConfig.zoneName}");
        }
    }
    
    // MEJORAR este método público para recibir la referencia del jugador
    public void ForcePlayerDetection(Transform playerRef = null)
    {
        // Si nos pasan una referencia, usarla
        if (playerRef != null)
        {
            playerTransform = playerRef;
        }
        // Si no tenemos al jugador, intentar encontrarlo
        else if (playerTransform == null && autoFindPlayer)
        {
            FindPlayer();
        }
        
        if (!isInitialized || playerTransform == null || currentZoneConfig == null) 
        {
            Debug.Log($"GlobalZoneSectorDetector: ForcePlayerDetection - No se puede forzar " +
                     $"(inicializado: {isInitialized}, jugador: {playerTransform != null}, zona: {currentZoneConfig != null})");
            return;
        }
        
        Debug.Log($"GlobalZoneSectorDetector: Forzando detección inicial en {currentZoneConfig.zoneName}");
        Debug.Log($"GlobalZoneSectorDetector: Posición del jugador: {playerTransform.position}");
        
        // Resetear estado actual para forzar nueva detección
        currentSectorGrid = new Vector2Int(-1, -1);
        currentSectorNumber = -1;
        
        // Verificar inmediatamente
        CheckPlayerSector();
    }
    
    // API pública para acceso externo
    public bool IsSectorRevealed(int zoneNumber, int sectorNumber)
    {
        if (!globalRevealedSectors.ContainsKey(zoneNumber)) return false;
        
        var config = System.Array.Find(allZoneConfigs, c => c.zoneNumber == zoneNumber);
        if (config == null) return false;
        
        Vector2Int gridPos = config.SectorNumberToGrid(sectorNumber);
        if (gridPos.x < 0 || gridPos.y < 0) return false;
        
        return globalRevealedSectors[zoneNumber][gridPos.x, gridPos.y];
    }
    
    public void ResetAllZones()
    {
        foreach (var kvp in globalRevealedSectors)
        {
            var array = kvp.Value;
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = false;
                }
            }
        }
        
        Debug.Log("GlobalZoneSectorDetector: Todas las zonas han sido reiniciadas");
    }
    
    public void ResetZone(int zoneNumber)
    {
        if (globalRevealedSectors.ContainsKey(zoneNumber))
        {
            var array = globalRevealedSectors[zoneNumber];
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = false;
                }
            }
            
            Debug.Log($"GlobalZoneSectorDetector: Zona {zoneNumber} reiniciada");
        }
    }
    
    // AGREGAR: Método para encontrar configuración por escena
    public ZoneConfiguration FindConfigForScene(string sceneName)
    {
        foreach (var config in allZoneConfigs)
        {
            if (config != null && config.zoneScene != null && config.zoneScene.SceneName == sceneName)
            {
                Debug.Log($"🎯 Configuración encontrada para escena '{sceneName}': {config.zoneName}");
                return config;
            }
        }
        
        Debug.LogWarning($"⚠️ No se encontró configuración para escena: {sceneName}");
        return null;
    }
    
    // Propiedades públicas
    public ZoneConfiguration CurrentZoneConfig => currentZoneConfig;
    public int CurrentSectorNumber => currentSectorNumber;
    public Vector2Int CurrentSectorGrid => currentSectorGrid;
    public bool IsInitialized => isInitialized;
    
    // Gizmos de debug
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || !isInitialized || currentZoneConfig == null) return;
        
        // Dibujar grid de la zona actual
        Gizmos.color = Color.yellow;
        Vector2 sectorSize = currentZoneConfig.SectorSize;
        
        // Líneas del grid
        for (int x = 0; x <= currentZoneConfig.gridWidth; x++)
        {
            Vector3 start = new Vector3(
                currentZoneConfig.worldMinBounds.x + x * sectorSize.x, 
                currentZoneConfig.worldMinBounds.y, 0);
            Vector3 end = new Vector3(
                currentZoneConfig.worldMinBounds.x + x * sectorSize.x, 
                currentZoneConfig.worldMaxBounds.y, 0);
            Gizmos.DrawLine(start, end);
        }
        
        for (int y = 0; y <= currentZoneConfig.gridHeight; y++)
        {
            Vector3 start = new Vector3(
                currentZoneConfig.worldMinBounds.x, 
                currentZoneConfig.worldMinBounds.y + y * sectorSize.y, 0);
            Vector3 end = new Vector3(
                currentZoneConfig.worldMaxBounds.x, 
                currentZoneConfig.worldMinBounds.y + y * sectorSize.y, 0);
            Gizmos.DrawLine(start, end);
        }
        
        // Dibujar sectores revelados
        if (globalRevealedSectors.ContainsKey(currentZoneConfig.zoneNumber))
        {
            Gizmos.color = Color.green;
            var revealedSectors = globalRevealedSectors[currentZoneConfig.zoneNumber];
            
            for (int x = 0; x < currentZoneConfig.gridWidth; x++)
            {
                for (int y = 0; y < currentZoneConfig.gridHeight; y++)
                {
                    if (revealedSectors[x, y])
                    {
                        Vector3 center = new Vector3(
                            currentZoneConfig.worldMinBounds.x + (x + 0.5f) * sectorSize.x,
                            currentZoneConfig.worldMaxBounds.y - (y + 0.5f) * sectorSize.y, 0);
                        Gizmos.DrawWireCube(center, new Vector3(sectorSize.x * 0.9f, sectorSize.y * 0.9f, 0.1f));
                    }
                }
            }
        }
        
        // Dibujar jugador
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, 0.5f);
            
            #if UNITY_EDITOR
            if (currentSectorNumber > 0)
            {
                Vector3 textPos = playerTransform.position + Vector3.up * 1.5f;
                UnityEditor.Handles.Label(textPos, $"Sector {currentSectorNumber}");
            }
            #endif
        }
    }
}