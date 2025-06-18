// ZoneConfiguration.cs - ScriptableObject para configurar cada zona con SceneField
using UnityEngine;

[CreateAssetMenu(fileName = "NewZoneConfig", menuName = "Mapping/Zone Configuration")]
public class ZoneConfiguration : ScriptableObject
{
    [Header("Zone Settings")]
    public int zoneNumber = 1;
    public string zoneName = "Zone 1";
    public SceneField zoneScene; // Referencia a la escena usando tu SceneField
    
    [Header("Grid Configuration")]
    public int gridWidth = 2;   // Sectores horizontales
    public int gridHeight = 2;  // Sectores verticales
    
    [Header("World Bounds")]
    [Tooltip("Coordenadas del mundo donde empieza la zona")]
    public Vector2 worldMinBounds = new Vector2(-10, -10);
    [Tooltip("Coordenadas del mundo donde termina la zona")]
    public Vector2 worldMaxBounds = new Vector2(10, 10);
    
    [Header("Camera Configuration")]
    [Tooltip("Solo usa esto si esta zona necesita un prefab MUY específico. Deja vacío para usar el prefab por defecto.")]
    public GameObject virtualCameraPrefab; // ✅ Opcional para casos especiales
    
    [Header("Camera Overrides (Opcional)")]
    [Tooltip("Marca esto para personalizar la cámara de esta zona específica.")]
    public bool overrideCameraSettings = false;
    
    [Space(5)]
    [Tooltip("El tamaño ortográfico de la cámara para esta zona.")]
    public float cameraSize = 8.0f;
    
    [Tooltip("La amortiguación (damping) de la cámara en X e Y.")]
    public float cameraDamping = 0.5f;
    
    [Tooltip("Dead zone horizontal (0-1).")]
    [Range(0f, 1f)] public float deadZoneWidth = 0.1f;
    
    [Tooltip("Dead zone vertical (0-1).")]
    [Range(0f, 1f)] public float deadZoneHeight = 0.1f;
    
    [Header("Visual Settings")]
    public Color sectorColor = Color.cyan;
    public Color unrevealedColor = Color.gray;

    // NEW: Configuración del icono del jugador
    [Space(5)]
    [Header("Player Icon Settings")]
    [Tooltip("Color del icono del jugador en esta zona")]
    public Color playerIconColor = Color.white;
    [Tooltip("¿El icono del jugador debe parpadear en esta zona?")]
    public bool enablePlayerIconBlinking = true;
    [Tooltip("Velocidad del parpadeo del icono del jugador")]
    [Range(0.5f, 5f)]
    public float playerIconBlinkSpeed = 2f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool showDebugGizmos = true;
    
    // Propiedades calculadas
    public int TotalSectors => gridWidth * gridHeight;
    public Vector2 SectorSize => new Vector2(
        (worldMaxBounds.x - worldMinBounds.x) / gridWidth,
        (worldMaxBounds.y - worldMinBounds.y) / gridHeight
    );
    
    // Verificar si esta configuración corresponde a la escena actual
    public bool IsCurrentScene()
    {
        if (zoneScene == null || string.IsNullOrEmpty(zoneScene.SceneName))
            return false;
            
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == zoneScene.SceneName;
    }
    
    // Convertir coordenadas del mundo a índices de grid
    public Vector2Int WorldToGridPosition(Vector2 worldPos)
    {
        // Verificar si está dentro de bounds
        if (worldPos.x < worldMinBounds.x || worldPos.x > worldMaxBounds.x ||
            worldPos.y < worldMinBounds.y || worldPos.y > worldMaxBounds.y)
        {
            return new Vector2Int(-1, -1); // Fuera de bounds
        }
        
        float normalizedX = (worldPos.x - worldMinBounds.x) / (worldMaxBounds.x - worldMinBounds.x);
        float normalizedY = (worldPos.y - worldMinBounds.y) / (worldMaxBounds.y - worldMinBounds.y);
        
        int gridX = Mathf.Clamp(Mathf.FloorToInt(normalizedX * gridWidth), 0, gridWidth - 1);
        int gridY = Mathf.Clamp(Mathf.FloorToInt((1f - normalizedY) * gridHeight), 0, gridHeight - 1); // Invertir Y
        
        return new Vector2Int(gridX, gridY);
    }
    
    // Convertir coordenadas de grid a número de sector (izquierda a derecha, arriba a abajo)
    public int GridToSectorNumber(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0) return -1;
        return gridPos.y * gridWidth + gridPos.x + 1; // +1 para empezar en 1
    }
    
    // Convertir número de sector a coordenadas de grid
    public Vector2Int SectorNumberToGrid(int sectorNumber)
    {
        if (sectorNumber < 1 || sectorNumber > TotalSectors) 
            return new Vector2Int(-1, -1);
            
        int index = sectorNumber - 1; // -1 porque empezamos en 1
        return new Vector2Int(index % gridWidth, index / gridWidth);
    }
    
    // Obtener posición del mundo del centro de un sector
    public Vector2 GetSectorWorldCenter(int sectorNumber)
    {
        Vector2Int gridPos = SectorNumberToGrid(sectorNumber);
        if (gridPos.x < 0 || gridPos.y < 0) return Vector2.zero;
        
        Vector2 sectorSize = SectorSize;
        
        float centerX = worldMinBounds.x + (gridPos.x + 0.5f) * sectorSize.x;
        float centerY = worldMaxBounds.y - (gridPos.y + 0.5f) * sectorSize.y; // Invertir Y
        
        return new Vector2(centerX, centerY);
    }
    
    // Obtener bounds de un sector específico
    public Bounds GetSectorBounds(int sectorNumber)
    {
        Vector2Int gridPos = SectorNumberToGrid(sectorNumber);
        if (gridPos.x < 0 || gridPos.y < 0) return new Bounds();
        
        Vector2 sectorSize = SectorSize;
        Vector2 center = GetSectorWorldCenter(sectorNumber);
        
        return new Bounds(center, new Vector3(sectorSize.x, sectorSize.y, 0));
    }
    
    // Validar configuración
    private void OnValidate()
    {
        if (gridWidth < 1) gridWidth = 1;
        if (gridHeight < 1) gridHeight = 1;
        
        if (worldMaxBounds.x <= worldMinBounds.x)
            worldMaxBounds.x = worldMinBounds.x + 1;
        if (worldMaxBounds.y <= worldMinBounds.y)
            worldMaxBounds.y = worldMinBounds.y + 1;
            
        if (string.IsNullOrEmpty(zoneName) && zoneScene != null)
            zoneName = zoneScene.SceneName;
    }
}