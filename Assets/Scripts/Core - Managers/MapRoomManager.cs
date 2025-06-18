using System.Collections.Generic;
using UnityEngine;

public class MapRoomManager : MonoBehaviour 
{
    // RESTAURADO: Singleton Pattern
    public static MapRoomManager Instance { get; private set; }
    
    [Header("Existing MapRoom System")]
    [SerializeField] private Transform mapRoomContainer;
    private Dictionary<string, MapContainerData> _mapRooms = new Dictionary<string, MapContainerData>();
    
    [Header("NEW: Zone Configurations")]
    [SerializeField] private ZoneConfiguration[] zoneConfigs;
    
    [Header("NEW: Visual Generation")]
    [SerializeField] private GameObject sectorVisualPrefab;
    
    [Header("NEW: Map Camera Centering")]
    [SerializeField] private Camera mapCamera; // Cámara dedicada al minimapa UI
    [SerializeField] private float cameraTransitionSpeed = 3f;
    [SerializeField] private bool enableMapCentering = true;
    
    // Configuración específica para minimapa
    private const string MAP_CAMERA_NAME = "Map Camera";
    private Vector3 _targetCameraPosition;
    
    // NEW: Dictionary para los sectores visuales
    private Dictionary<string, List<MapSectorVisual>> _zoneSectors = 
        new Dictionary<string, List<MapSectorVisual>>();
    
    // Current room tracking
    private string _currentRoomName;
    
    // RESTAURADO: Awake para Singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        InitializeMapRooms();
        GenerateMapVisuals();
        
        // Buscar la cámara del minimapa
        FindMapCamera();
    }
    
    // CORREGIDO: Buscar cámara UI del minimapa
    private void FindMapCamera()
    {
        if (mapCamera != null) 
        {
            Debug.Log($"✅ Map Camera ya asignada: {mapCamera.name}");
            return;
        }
        
        // Buscar por nombre exacto "Map Camera"
        GameObject mapCameraObj = GameObject.Find(MAP_CAMERA_NAME);
        if (mapCameraObj != null)
        {
            Camera cam = mapCameraObj.GetComponent<Camera>();
            if (cam != null)
            {
                mapCamera = cam;
                Debug.Log($"✅ Map Camera encontrada: {mapCamera.name}");
                return;
            }
            else
            {
                Debug.LogWarning($"⚠️ GameObject '{MAP_CAMERA_NAME}' encontrado pero no tiene componente Camera");
            }
        }
        
        // Búsqueda alternativa entre todas las cámaras
        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (var cam in allCameras)
        {
            if (cam.name.Equals(MAP_CAMERA_NAME, System.StringComparison.OrdinalIgnoreCase))
            {
                mapCamera = cam;
                Debug.Log($"✅ Map Camera encontrada (búsqueda alternativa): {mapCamera.name}");
                return;
            }
        }
        
        Debug.LogWarning($"❌ No se encontró Camera con nombre '{MAP_CAMERA_NAME}'. Asígnala manualmente en el inspector.");
    }
    
    private void Update()
    {
        // SIMPLE: Solo mover la cámara del minimapa hacia el objetivo
        if (enableMapCentering && mapCamera != null)
        {
            Vector3 currentPos = mapCamera.transform.position;
            Vector3 targetPos = new Vector3(_targetCameraPosition.x, _targetCameraPosition.y, currentPos.z);
            
            float distance = Vector3.Distance(currentPos, targetPos);
            if (distance > 0.01f)
            {
                Vector3 newPos = Vector3.Lerp(currentPos, targetPos, cameraTransitionSpeed * Time.deltaTime);
                mapCamera.transform.position = newPos;
            }
        }
    }
    
    // CORREGIDO: Usar MapContainerData en lugar de MapRoom
    private void InitializeMapRooms()
    {
        MapContainerData[] rooms = mapRoomContainer.GetComponentsInChildren<MapContainerData>();
        foreach (var room in rooms)
        {
            if (room.roomScene != null && !string.IsNullOrEmpty(room.roomScene.SceneName))
            {
                _mapRooms[room.roomScene.SceneName] = room;
            }
        }
    }
    
    // RESTAURADO: Método que usa GameManager
    public void ResetAllZones()
    {
        // Reset MapContainerData revelations
        foreach (var room in _mapRooms.Values)
        {
            room.HasBeenRevealed = false;
        }
        
        // NEW: Reset zone sector revelations
        foreach (var zoneSectors in _zoneSectors.Values)
        {
            foreach (var sector in zoneSectors)
            {
                sector.HasBeenRevealed = false;
                sector.UpdateVisualState();
                sector.SetPlayerPresence(false);
            }
        }
        
        Debug.Log("All zones and sectors have been reset");
    }
    
    // RESTAURADO: Método que busca ZoneSectorDetector
    public void RegisterZone(ZoneConfiguration zoneConfig)
    {
        // Registrar la zona en el sistema existente
        // (Preservar funcionalidad original que usa ZoneSectorDetector)
        Debug.Log($"Zone {zoneConfig.zoneName} registered with {zoneConfig.TotalSectors} sectors");
    }
    
    // RESTAURADO: Método que llama ZoneSectorDetector
    public void RevealZoneSector(int zoneNumber, int sectorNumber)
    {
        // Buscar la configuración de zona por número
        ZoneConfiguration targetZone = null;
        foreach (var zoneConfig in zoneConfigs)
        {
            if (zoneConfig.zoneNumber == zoneNumber)
            {
                targetZone = zoneConfig;
                break;
            }
        }
        
        if (targetZone == null)
        {
            Debug.LogWarning($"Zone {zoneNumber} not found in zone configurations");
            return;
        }
        
        // Llamar al método existente con el nombre de zona
        OnPlayerEnteredSector(targetZone.zoneName, sectorNumber);
    }
    
    // NEW: Generar visuales para cada zona
    private void GenerateMapVisuals()
    {
        foreach (var zoneConfig in zoneConfigs)
        {
            CreateZoneVisuals(zoneConfig);
        }
    }
    
    // NEW: Crear sectores visuales para una zona
    private void CreateZoneVisuals(ZoneConfiguration zoneConfig)
    {
        List<MapSectorVisual> sectorList = new List<MapSectorVisual>();
        
        // Crear visual para cada sector según grid (1 to TotalSectors)
        for (int sectorNum = 1; sectorNum <= zoneConfig.TotalSectors; sectorNum++)
        {
            GameObject sectorObj = Instantiate(sectorVisualPrefab, mapRoomContainer);
            MapSectorVisual sectorVisual = sectorObj.GetComponent<MapSectorVisual>();
            
            if (sectorVisual != null)
            {
                sectorVisual.InitializeSector(zoneConfig, sectorNum);
                sectorList.Add(sectorVisual);
            }
        }
        
        _zoneSectors[zoneConfig.zoneName] = sectorList;
    }
    
    // Método existente - actualizar sala actual
    public virtual void UpdateCurrentRoom(string sceneName)
    {
        _currentRoomName = sceneName;
        
        if (_mapRooms.ContainsKey(sceneName))
        {
            Debug.Log($"Current room updated to: {sceneName}");
        }
        else
        {
            Debug.LogWarning($"Room {sceneName} not found in map rooms dictionary");
        }
    }
    
    // NEW: Llamado desde ZoneSectorDetector cuando el jugador entra a un sector
    public void OnPlayerEnteredSector(string zoneName, int sectorNumber)
    {
        if (!_zoneSectors.ContainsKey(zoneName)) return;
        
        var zoneSectors = _zoneSectors[zoneName];
        
        // Revelar sector actual
        var currentSector = zoneSectors.Find(s => s.sectorNumber == sectorNumber);
        if (currentSector != null)
        {
            currentSector.HasBeenRevealed = true;
            currentSector.UpdateVisualState();
            
            Debug.Log($"Revealed sector {sectorNumber} in {zoneName}");
        }
        
        // Actualizar posición del jugador Y centrar cámara
        UpdatePlayerPosition(zoneName, sectorNumber);
    }
    
    public void UpdatePlayerPosition(string zoneName, int sectorNumber)
    {
        // Limpiar posición anterior del jugador en TODOS los sectores de TODAS las zonas
        foreach (var zoneSectorList in _zoneSectors.Values)
        {
            foreach (var visual in zoneSectorList)
            {
                visual.SetPlayerPresence(false);
            }
        }
        
        // Si hay un sector válido, establecer jugador ahí
        if (sectorNumber > 0 && _zoneSectors.ContainsKey(zoneName))
        {
            var zoneSectors = _zoneSectors[zoneName];
            var targetVisual = zoneSectors.Find(v => v.sectorNumber == sectorNumber);
        
            if (targetVisual != null)
            {
                targetVisual.SetPlayerPresence(true);
                
                // NEW: Centrar cámara en el sector del jugador
                if (enableMapCentering)
                {
                    CenterCameraOnSector(targetVisual);
                }
                
                Debug.Log($"Jugador posicionado en sector {sectorNumber} de {zoneName}");
            }
            else
            {
                Debug.LogWarning($"No se encontró sector {sectorNumber} en zona {zoneName}");
            }
        }
    }
    
    // SIMPLE: Centrar cámara del minimapa
    private void CenterCameraOnSector(MapSectorVisual sector)
    {
        if (sector == null) 
        {
            Debug.LogWarning("🚫 CenterCameraOnSector: sector es null");
            return;
        }
        
        if (mapCamera == null)
        {
            Debug.LogWarning("🚫 CenterCameraOnSector: Map Camera no está asignada");
            FindMapCamera();
            if (mapCamera == null) return;
        }
        
        _targetCameraPosition = sector.transform.position;
        
        Debug.Log($"📍 MINIMAPA CENTRADO: Objetivo={_targetCameraPosition}, Sector={sector.sectorNumber}");
    }
    
    // NUEVO: Método para centrar inmediatamente (sin animación)
    public void CenterMapCameraImmediate(Vector3 position)
    {
        if (mapCamera != null)
        {
            Vector3 newPos = new Vector3(position.x, position.y, mapCamera.transform.position.z);
            mapCamera.transform.position = newPos;
            _targetCameraPosition = newPos;
        
            Debug.Log($"📍 MINIMAPA CENTRADO INMEDIATO: {newPos}");
        }
    }
    
    // MEJORADO: Debug específico para minimapa
    [ContextMenu("Debug Map Camera")]
    public void DebugMapCamera()
    {
        Debug.Log("=== ESTADO DE MAP CAMERA (MINIMAPA) ===");
        Debug.Log($"Buscando: '{MAP_CAMERA_NAME}'");
        Debug.Log($"Camera asignada: {mapCamera != null}");
        
        if (mapCamera != null)
        {
            Debug.Log($"Nombre: '{mapCamera.name}'");
            Debug.Log($"Posición actual: {mapCamera.transform.position}");
            Debug.Log($"Posición objetivo: {_targetCameraPosition}");
            Debug.Log($"Render Texture: {mapCamera.targetTexture != null}");
            Debug.Log($"Culling Mask: {mapCamera.cullingMask}");
            Debug.Log($"Depth: {mapCamera.depth}");
        }
        else
        {
            Debug.LogWarning($"❌ '{MAP_CAMERA_NAME}' no encontrada");
        
            // Mostrar todas las cámaras disponibles
            Camera[] allCameras = FindObjectsOfType<Camera>();
            Debug.Log("🔍 Cámaras disponibles:");
            
            foreach (var cam in allCameras)
            {
                Debug.Log($"   - '{cam.name}' (escena: {cam.gameObject.scene.name})");
            }
        }
        
        Debug.Log($"Centrado habilitado: {enableMapCentering}");
        Debug.Log("=====================================");
    }
    
    // NUEVO: Método público para asignar manualmente
    public void AssignMapCamera(Camera newMapCamera)
    {
        if (newMapCamera != null)
        {
            mapCamera = newMapCamera;
            Debug.Log($"🎥 Map Camera asignada manualmente: {newMapCamera.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ La cámara asignada es null");
        }
    }
    
    // MEJORADO: Centrar con información del minimapa
    public void CenterOnPlayerPosition()
    {
        foreach (var zoneSectorList in _zoneSectors.Values)
        {
            foreach (var visual in zoneSectorList)
            {
                if (visual.IsPlayerIconActive())
                {
                    CenterCameraOnSector(visual);
                    return;
                }
            }
        }
        
        Debug.LogWarning("🔍 No se encontró la posición actual del jugador en el minimapa");
    }
    
    // NUEVO: Método para obtener la cámara del minimapa
    public Camera GetMapCamera()
    {
        return mapCamera;
    }
    
    // NUEVO: Verificar si la cámara del minimapa está configurada correctamente
    public bool IsMapCameraValid()
    {
        return mapCamera != null && mapCamera.enabled;
    }

    // Añadir este nuevo método después del método CenterOnPlayerPosition() existente

    /// <summary>
    /// Centra una cámara específica en la posición actual del jugador (para mapa grande)
    /// </summary>
    /// <param name="cameraToCenter">La cámara que se va a centrar</param>
    public void CenterCameraOnPlayer(Camera cameraToCenter)
    {
        if (cameraToCenter == null)
        {
            Debug.LogWarning("Se intentó centrar una cámara nula.");
            return;
        }

        // Buscamos el sector visual donde está el jugador
        foreach (var zoneSectorList in _zoneSectors.Values)
        {
            foreach (var visual in zoneSectorList)
            {
                if (visual.IsPlayerHere())
                {
                    // Movemos la cámara que nos pasaron a la posición de ese sector
                    Vector3 targetPosition = visual.transform.position;
                    cameraToCenter.transform.position = new Vector3(targetPosition.x, targetPosition.y, cameraToCenter.transform.position.z);
                    Debug.Log($"Mapa grande centrado instantáneamente en el sector: {visual.name}");
                    return;
                }
            }
        }
        
        Debug.LogWarning("No se encontró la posición del jugador en el mapa para centrar la cámara.");
    }
}