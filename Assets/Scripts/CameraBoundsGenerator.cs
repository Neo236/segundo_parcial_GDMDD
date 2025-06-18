using UnityEngine;
using System;
using System.Collections;
using Unity.Cinemachine; // ✅ CORRECTO para Cinemachine v3

public class CameraBoundsGenerator : MonoBehaviour
{
    public static CameraBoundsGenerator Instance { get; private set; }
    
    [Header("Camera Bounds Settings")]
    [SerializeField] private float boundsExpansion = 2f;
    [SerializeField] private bool debugBounds = true;
    
    [Header("Auto-Assignment Results")]
    [SerializeField] private bool lastAutoAssignmentSuccessful = false;
    [SerializeField] private string lastStatusMessage = "Esperando zona...";
    
    private PolygonCollider2D currentBoundsCollider;
    private Component confinerComponent;
    private string lastGeneratedZoneName = "";
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠️ Múltiples CameraBoundsGenerator detectados. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ CameraBoundsGenerator Singleton inicializado");
    }
    
    // ✅ MÉTODO PRINCIPAL ACTUALIZADO para v3
    public void GenerateAndAssignBounds(ZoneConfiguration zoneConfig, CinemachineCamera targetVCam)
    {
        if (zoneConfig == null || targetVCam == null)
        {
            Debug.LogError("❌ Se intentó generar bounds sin una ZoneConfiguration o una CinemachineCamera válidas.");
            return;
        }

        Debug.Log($"🚀 Petición del GameManager: Generando bounds para {zoneConfig.zoneName} en la cámara {targetVCam.name}");
        
        CleanupPreviousBounds();
        CreateBoundsCollider(zoneConfig);
        
        // Asignamos los bounds a la cámara específica que nos ha pasado el GameManager
        AttemptAutoAssignment(targetVCam); 
        
        ShowResults(zoneConfig, targetVCam);
        lastGeneratedZoneName = zoneConfig.zoneName;
    }
    
    // MÉTODO LEGACY: Para mantener compatibilidad (actualizado a v3)
    public void GenerateBoundsForZone(ZoneConfiguration zoneConfig)
    {
        // ✅ Buscar CinemachineCamera en lugar de CinemachineVirtualCamera
        var vcam = FindObjectOfType<CinemachineCamera>();
        if (vcam != null)
        {
            GenerateAndAssignBounds(zoneConfig, vcam);
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró CinemachineCamera en la escena para GenerateBoundsForZone");
        }
    }
    
    private void CleanupPreviousBounds()
    {
        if (currentBoundsCollider != null)
        {
            Debug.Log("🧹 Limpiando bounds anteriores...");
            DestroyImmediate(currentBoundsCollider.gameObject);
        }
    }
    
    private void CreateBoundsCollider(ZoneConfiguration zoneConfig)
    {
        // Crear nuevo GameObject para los bounds
        GameObject boundsObject = new GameObject($"CameraBounds_{zoneConfig.zoneName}");
        boundsObject.transform.SetParent(transform);
        
        // Agregar PolygonCollider2D
        currentBoundsCollider = boundsObject.AddComponent<PolygonCollider2D>();
        currentBoundsCollider.isTrigger = true;
        
        // Calcular puntos del polígono con expansión
        Vector2 min = zoneConfig.worldMinBounds - Vector2.one * boundsExpansion;
        Vector2 max = zoneConfig.worldMaxBounds + Vector2.one * boundsExpansion;
        
        // Crear rectángulo
        Vector2[] points = new Vector2[]
        {
            new Vector2(min.x, min.y), // Bottom-left
            new Vector2(max.x, min.y), // Bottom-right
            new Vector2(max.x, max.y), // Top-right
            new Vector2(min.x, max.y)  // Top-left
        };
        
        currentBoundsCollider.points = points;
        
        Debug.Log($"📐 Bounds creados: Min({min.x:F1}, {min.y:F1}) Max({max.x:F1}, {max.y:F1})");
    }
    
    // ✅ MODIFICADO: AttemptAutoAssignment para v3
    private void AttemptAutoAssignment(CinemachineCamera vcam)
    {
        lastAutoAssignmentSuccessful = false;
        lastStatusMessage = "🔄 Iniciando auto-asignación...";
        
        // Verificar CinemachineCamera
        if (vcam == null)
        {
            lastStatusMessage = "❌ CinemachineCamera no asignada";
            Debug.LogWarning(lastStatusMessage);
            return;
        }
        
        // Intentar encontrar el Confiner2D en la cámara específica
        if (!FindConfinerComponent(vcam))
        {
            return;
        }
        
        // Intentar asignar usando reflexión
        if (!AssignUsingReflection())
        {
            return;
        }
        
        // ¡Éxito!
        lastAutoAssignmentSuccessful = true;
        lastStatusMessage = "✅ Auto-asignación exitosa";
        Debug.Log($"✅ Bounds asignados automáticamente al Confiner de {vcam.name}");
    }
    
    // ✅ ACTUALIZADO para v3
    private bool FindConfinerComponent(CinemachineCamera vcam)
    {
        // Buscar el confiner en la cámara específica
        confinerComponent = vcam.GetComponent<CinemachineConfiner2D>();
        
        if (confinerComponent == null)
        {
            // Intentar con reflection si no funciona la referencia directa
            confinerComponent = vcam.GetComponent("CinemachineConfiner2D");
        }
        
        if (confinerComponent == null)
        {
            lastStatusMessage = $"❌ CinemachineConfiner2D no encontrado en {vcam.name}";
            Debug.LogWarning(lastStatusMessage);
            return false;
        }
        
        Debug.Log($"📷 CinemachineConfiner2D encontrado en {vcam.name}: {confinerComponent.GetType().Name}");
        return true;
    }
    
    private bool AssignUsingReflection()
    {
        try
        {
            // Buscar múltiples nombres de campo posibles
            string[] possibleFieldNames = {
                "m_BoundingShape2D",
                "BoundingShape2D", 
                "boundingShape2D",
                "m_BoundingShape"
            };
            
            System.Reflection.FieldInfo boundingShapeField = null;
            
            foreach (string fieldName in possibleFieldNames)
            {
                boundingShapeField = confinerComponent.GetType().GetField(fieldName, 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                
                if (boundingShapeField != null)
                {
                    Debug.Log($"🔍 Campo encontrado: {fieldName}");
                    break;
                }
            }
            
            if (boundingShapeField == null)
            {
                lastStatusMessage = "❌ Campo de bounding shape no encontrado";
                Debug.LogWarning(lastStatusMessage);
                return false;
            }
            
            // Asignar el collider
            boundingShapeField.SetValue(confinerComponent, currentBoundsCollider);
            Debug.Log("🔗 Campo asignado exitosamente");
            
            // Intentar invalidar cache si existe
            TryInvalidateCache();
            
            return true;
        }
        catch (Exception ex)
        {
            lastStatusMessage = $"❌ Error en reflexión: {ex.Message}";
            Debug.LogWarning(lastStatusMessage);
            return false;
        }
    }
    
    private void TryInvalidateCache()
    {
        try
        {
            string[] possibleMethodNames = {
                "InvalidateCache",
                "InvalidatePathCache", 
                "OnValidate",
                "RefreshCache"
            };
            
            foreach (string methodName in possibleMethodNames)
            {
                var method = confinerComponent.GetType().GetMethod(methodName, 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                
                if (method != null)
                {
                    method.Invoke(confinerComponent, null);
                    Debug.Log($"🔄 {methodName} ejecutado");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"⚠️ Error al invalidar cache: {ex.Message}");
        }
    }
    
    // ✅ ACTUALIZADO para v3
    private void ShowResults(ZoneConfiguration zoneConfig, CinemachineCamera vcam)
    {
        Debug.Log("═══════════════════════════════════════");
        Debug.Log($"🎯 RESULTADO PARA ZONA: {zoneConfig.zoneName}");
        Debug.Log($"📷 CÁMARA: {vcam.name}");
        Debug.Log($"📊 Estado: {lastStatusMessage}");
        
        if (lastAutoAssignmentSuccessful)
        {
            Debug.Log("🎉 ¡TODO LISTO! Los bounds se asignaron automáticamente.");
        }
        else
        {
            Debug.Log("⚙️ ASIGNACIÓN MANUAL REQUERIDA:");
            Debug.Log($"   1. Ve a tu CinemachineCamera: '{vcam.name}'");
            Debug.Log($"   2. En el componente 'CinemachineConfiner2D'");
            Debug.Log($"   3. Arrastra '{currentBoundsCollider.name}' al campo 'Bounding Shape 2D'");
        }
        Debug.Log("═══════════════════════════════════════");
    }
    
    // Método para debug visual
    private void OnDrawGizmos()
    {
        if (debugBounds && currentBoundsCollider != null)
        {
            Gizmos.color = lastAutoAssignmentSuccessful ? Color.green : Color.yellow;
            
            Vector2[] points = currentBoundsCollider.points;
            
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 current = transform.TransformPoint(points[i]);
                Vector3 next = transform.TransformPoint(points[(i + 1) % points.Length]);
                Gizmos.DrawLine(current, next);
            }
            
            #if UNITY_EDITOR
            Vector3 center = transform.TransformPoint(Vector3.zero);
            string statusText = lastAutoAssignmentSuccessful ? "AUTO ✅" : "MANUAL ⚙️";
            UnityEditor.Handles.Label(center, $"{statusText}\n{lastGeneratedZoneName}");
            #endif
        }
    }
}