// ZoneSectorDetector.cs - Detector automático que se auto-configura basado en la escena actual
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneSectorDetector : MonoBehaviour
{
    [Header("Zone Configuration")]
    [Tooltip("Todas las configuraciones de zonas del juego")]
    public ZoneConfiguration[] allZoneConfigs;
    
    [Header("Player Detection")]
    public Transform playerTransform;
    public string playerTag = "Player";
    
    [Header("Detection Settings")]
    public float detectionInterval = 0.1f; // Verificar cada 0.1 segundos
    public bool autoFindPlayer = true;
    
    [Header("Debug")]
    public bool showDebugGizmos = true;
    public bool showDebugInfo = false;
    
    // Estado actual
    private ZoneConfiguration currentZoneConfig;
    private bool[,] revealedSectors;
    private Vector2Int currentSectorGrid = new Vector2Int(-1, -1);
    private int currentSectorNumber = -1;
    private float lastDetectionTime = 0f;
    private bool isInitialized = false;
    
    private void Start()
    {
        InitializeForCurrentScene();
    }
    
    private void InitializeForCurrentScene()
    {
        // Buscar la configuración que corresponde a la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        foreach (ZoneConfiguration config in allZoneConfigs)
        {
            if (config != null && config.zoneScene != null && 
                config.zoneScene.SceneName == currentSceneName)
            {
                currentZoneConfig = config;
                break;
            }
        }
        
        if (currentZoneConfig == null)
        {
            Debug.LogWarning($"No se encontró configuración para la escena '{currentSceneName}'");
            return;
        }
        
        // Inicializar array de sectores revelados
        revealedSectors = new bool[currentZoneConfig.gridWidth, currentZoneConfig.gridHeight];
        
        // Buscar jugador si está configurado para auto-buscar
        if (autoFindPlayer && playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
                playerTransform = player.transform;
        }
        
        // CORREGIDO: Registrar la ZoneConfiguration, no this
        if (MapRoomManager.Instance != null)
        {
            MapRoomManager.Instance.RegisterZone(currentZoneConfig);
        }
        
        isInitialized = true;
        
        if (currentZoneConfig.showDebugInfo)
        {
            Debug.Log($"ZoneSectorDetector inicializado para {currentZoneConfig.zoneName}: " +
                     $"{currentZoneConfig.gridWidth}x{currentZoneConfig.gridHeight} sectores");
        }
    }
    
    private void Update()
    {
        if (!isInitialized || playerTransform == null || currentZoneConfig == null) return;
        
        // Verificar posición del jugador cada cierto intervalo
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
            if (currentZoneConfig.showDebugInfo && currentSectorNumber != -1)
            {
                Debug.Log("Jugador fuera de la zona de sectores");
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
            
            // Revelar el nuevo sector si no estaba revelado
            if (!revealedSectors[newGridPos.x, newGridPos.y])
            {
                RevealSector(newGridPos.x, newGridPos.y);
            }
            
            if (currentZoneConfig.showDebugInfo)
            {
                Debug.Log($"Jugador en sector {currentSectorNumber} (Grid: {newGridPos}) " +
                         $"de {currentZoneConfig.zoneName}");
            }
        }
    }
    
    public void RevealSector(int gridX, int gridY)
    {
        if (!isInitialized || currentZoneConfig == null) return;
        
        if (gridX < 0 || gridX >= currentZoneConfig.gridWidth || 
            gridY < 0 || gridY >= currentZoneConfig.gridHeight) return;
        
        revealedSectors[gridX, gridY] = true;
        int sectorNumber = currentZoneConfig.GridToSectorNumber(new Vector2Int(gridX, gridY));
        
        // Notificar al MapRoomManager
        if (MapRoomManager.Instance != null)
        {
            MapRoomManager.Instance.RevealZoneSector(currentZoneConfig.zoneNumber, sectorNumber);
        }
        
        if (currentZoneConfig.showDebugInfo)
        {
            Debug.Log($"Sector {sectorNumber} revelado en {currentZoneConfig.zoneName}");
        }
    }
    
    public void RevealSectorByNumber(int sectorNumber)
    {
        if (!isInitialized || currentZoneConfig == null) return;
        
        Vector2Int gridPos = currentZoneConfig.SectorNumberToGrid(sectorNumber);
        if (gridPos.x >= 0 && gridPos.y >= 0)
        {
            RevealSector(gridPos.x, gridPos.y);
        }
    }
    
    public bool IsSectorRevealed(int gridX, int gridY)
    {
        if (!isInitialized || currentZoneConfig == null) return false;
        
        if (gridX < 0 || gridX >= currentZoneConfig.gridWidth || 
            gridY < 0 || gridY >= currentZoneConfig.gridHeight) 
            return false;
        
        return revealedSectors[gridX, gridY];
    }
    
    public bool IsSectorRevealedByNumber(int sectorNumber)
    {
        if (!isInitialized || currentZoneConfig == null) return false;
        
        Vector2Int gridPos = currentZoneConfig.SectorNumberToGrid(sectorNumber);
        return IsSectorRevealed(gridPos.x, gridPos.y);
    }
    
    public void ResetZone()
    {
        if (!isInitialized || currentZoneConfig == null) return;
        
        revealedSectors = new bool[currentZoneConfig.gridWidth, currentZoneConfig.gridHeight];
        currentSectorGrid = new Vector2Int(-1, -1);
        currentSectorNumber = -1;
        
        if (currentZoneConfig.showDebugInfo)
        {
            Debug.Log($"Zona {currentZoneConfig.zoneName} reiniciada");
        }
    }
    
    // Propiedades públicas para acceso externo
    public ZoneConfiguration ZoneConfig => currentZoneConfig;
    public int CurrentSectorNumber => currentSectorNumber;
    public Vector2Int CurrentSectorGrid => currentSectorGrid;
    public bool IsInitialized => isInitialized;
    
    // Obtener array de sectores para persistencia
    public bool[,] GetRevealedSectorsArray()
    {
        if (!isInitialized || revealedSectors == null) return null;
        return (bool[,])revealedSectors.Clone();
    }
    
    // Establecer array de sectores desde persistencia
    public void SetRevealedSectorsArray(bool[,] sectors)
    {
        if (!isInitialized || currentZoneConfig == null) return;
        
        if (sectors != null && 
            sectors.GetLength(0) == currentZoneConfig.gridWidth && 
            sectors.GetLength(1) == currentZoneConfig.gridHeight)
        {
            revealedSectors = (bool[,])sectors.Clone();
            
            // Actualizar el mapa visual
            if (MapRoomManager.Instance != null)
            {
                for (int x = 0; x < currentZoneConfig.gridWidth; x++)
                {
                    for (int y = 0; y < currentZoneConfig.gridHeight; y++)
                    {
                        if (sectors[x, y])
                        {
                            int sectorNumber = currentZoneConfig.GridToSectorNumber(new Vector2Int(x, y));
                            MapRoomManager.Instance.RevealZoneSector(currentZoneConfig.zoneNumber, sectorNumber);
                        }
                    }
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || currentZoneConfig == null) return;
        
        // Dibujar grid de la zona
        Gizmos.color = Color.yellow;
        Vector2 sectorSize = currentZoneConfig.SectorSize;
        
        // Líneas verticales
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
        
        // Líneas horizontales
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
        if (Application.isPlaying && revealedSectors != null)
        {
            Gizmos.color = Color.green;
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
        
        // Dibujar posición actual del jugador si está disponible
        if (Application.isPlaying && playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, 0.5f);
            
            // Mostrar número de sector actual
            if (currentSectorNumber > 0)
            {
                Vector3 textPos = playerTransform.position + Vector3.up * 1.5f;
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(textPos, $"Sector {currentSectorNumber}");
                #endif
            }
        }
    }
}