using UnityEngine;
using System.Collections;

public class MapSectorVisual : MapContainerData 
{
    [Header("Visual Components")]
    [SerializeField] private SpriteRenderer sectorRenderer;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private SpriteRenderer playerIconRenderer; // NEW: Renderer del icono
    
    [Header("Player Icon Settings")]
    [SerializeField] private Color playerIconColor = Color.white;
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private bool enableBlinking = true;
    
    [Header("Sector Grid Info")]
    public int sectorNumber;
    public Vector2Int gridPosition;
    public ZoneConfiguration parentZone;
    
    private Coroutine blinkCoroutine;
    private bool isPlayerHere = false;
    
    private void Start()
    {
        // Asegurar layer "Map"
        gameObject.layer = LayerMask.NameToLayer("Map");
        
        // NEW: Auto-asignar playerIconRenderer si no está asignado
        if (playerIconRenderer == null && playerIcon != null)
        {
            playerIconRenderer = playerIcon.GetComponent<SpriteRenderer>();
        }
        
        // Inicializar como no revelado
        UpdateVisualState();
    }
    
    public void InitializeSector(ZoneConfiguration zone, int sector)
    {
        parentZone = zone;
        sectorNumber = sector;
        gridPosition = zone.SectorNumberToGrid(sector);
        roomScene = zone.zoneScene; // Usar el SceneField existente
        
        // NEW: Aplicar configuración de icono de la zona
        if (zone != null)
        {
            playerIconColor = zone.playerIconColor;
            enableBlinking = zone.enablePlayerIconBlinking;
            blinkSpeed = zone.playerIconBlinkSpeed;
            
            Debug.Log($"🎨 Sector {sector} de {zone.zoneName}: Color={zone.playerIconColor}, Parpadeo={zone.enablePlayerIconBlinking}");
        }
        
        // Posicionar en el mapa usando la grid lógica de ZoneConfiguration
        transform.position = CalculateMapPosition();
        gameObject.name = $"{zone.zoneName}_Sector_{sector}";
    }
    
    private Vector3 CalculateMapPosition()
    {
        if (parentZone == null) return Vector3.zero;
        
        // Usar las coordenadas de grid para posicionar en el mapa visual
        float mapTileSize = 1f; // Tamaño de cada tile en el mapa
        Vector3 zoneOffset = GetZoneOffset(parentZone.zoneName);
        
        float halfTile = mapTileSize / 2f;
        
        float baseX = zoneOffset.x + gridPosition.x * mapTileSize;
        float baseY = zoneOffset.y - gridPosition.y * mapTileSize;
        
        return new Vector3(
            baseX + halfTile,
            baseY - halfTile, // Y invertido (arriba→abajo)
            0f
        );
    }
    
    private Vector3 GetZoneOffset(string zoneName)
    {
        // Posicionar cada zona en el mapa global
        switch (zoneName)
        {
            case "Zone 1": return new Vector3(0f, 0f, 0f);
            case "Zone 2": return new Vector3(2f, -1f, 0f); // A la derecha-arriba de Zone 1
            case "Zone 3": return new Vector3(0f, -2f, 0f); // Abajo de Zone 1
            case "Zone 4": return new Vector3(2f, 0f, 0f); // A la derecha-abajo de Zone 1
            default: return Vector3.zero;
        }
    }
    
    public void UpdateVisualState()
    {
        if (HasBeenRevealed)
        {
            // Sector revelado - activar y mostrar
            gameObject.SetActive(true);
            if (sectorRenderer != null)
            {
                sectorRenderer.color = new Color(parentZone.sectorColor.r, parentZone.sectorColor.g, 
                                               parentZone.sectorColor.b, 1f);
            }
        }
        else
        {
            // Sector NO revelado - desactivar completamente
            gameObject.SetActive(false);
        }
    }
    
    // MEJORADO: Usar configuración de la zona
    public void SetPlayerPresence(bool isHere)
    {
        isPlayerHere = isHere;
        
        if (playerIcon != null)
        {
            bool shouldShow = isHere && HasBeenRevealed;
            playerIcon.SetActive(shouldShow);
            
            if (shouldShow)
            {
                // NEW: Usar configuración de la zona si está disponible
                Color iconColor = parentZone != null ? parentZone.playerIconColor : playerIconColor;
                bool shouldBlink = parentZone != null ? parentZone.enablePlayerIconBlinking : enableBlinking;
                float currentBlinkSpeed = parentZone != null ? parentZone.playerIconBlinkSpeed : blinkSpeed;
            
                // Aplicar color
                if (playerIconRenderer != null)
                {
                    playerIconRenderer.color = iconColor;
                }
                
                // Actualizar velocidad de parpadeo actual
                blinkSpeed = currentBlinkSpeed;
                
                // Aplicar parpadeo
                if (shouldBlink)
                {
                    StartBlinking();
                }
                else
                {
                    StopBlinking();
                }
                
                Debug.Log($"🎮 Jugador en sector {sectorNumber} de {parentZone.zoneName}: Color={iconColor}, Parpadeo={shouldBlink}, Velocidad={currentBlinkSpeed}");
            }
            else
            {
                StopBlinking();
            }
        }
    }
    
    // NEW: Iniciar efecto de parpadeo
    private void StartBlinking()
    {
        StopBlinking(); // Detener parpadeo anterior si existe
        
        if (playerIconRenderer != null)
        {
            blinkCoroutine = StartCoroutine(BlinkEffect());
        }
    }
    
    // NEW: Detener efecto de parpadeo
    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        
        // Asegurar que el icono siempre sea completamente visible cuando no está parpadeando
        if (playerIconRenderer != null)
        {
            Color color = playerIconRenderer.color;
            color.a = 1f; // ¡Siempre restaurar a alfa completo!
            playerIconRenderer.color = color;
        }
    }
    
    // NEW: Corrutina del efecto de parpadeo
    private IEnumerator BlinkEffect()
    {
        while (isPlayerHere && enableBlinking)
        {
            // Fade out
            yield return StartCoroutine(FadeIcon(1f, 0.3f));
            
            // Fade in
            yield return StartCoroutine(FadeIcon(0.3f, 1f));
        }
    }
    
    // NEW: Fade del icono
    private IEnumerator FadeIcon(float fromAlpha, float toAlpha)
    {
        if (playerIconRenderer == null) yield break;
        
        float elapsed = 0f;
        float duration = 1f / blinkSpeed;
        
        while (elapsed < duration)
        {
            // ¡USAR SIEMPRE tiempo no escalado para la UI!
            elapsed += Time.unscaledDeltaTime; 
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
            
            Color color = playerIconRenderer.color;
            color.a = alpha;
            playerIconRenderer.color = color;
            
            yield return null;
        }
        
        // Asegurar valor final
        Color finalColor = playerIconRenderer.color;
        finalColor.a = toAlpha;
        playerIconRenderer.color = finalColor;
    }
    
    // NEW: Métodos públicos para configurar el icono del jugador
    public void SetPlayerIconColor(Color newColor)
    {
        playerIconColor = newColor;
        
        if (playerIconRenderer != null && isPlayerHere)
        {
            Color color = playerIconRenderer.color;
            color.r = newColor.r;
            color.g = newColor.g;
            color.b = newColor.b;
            playerIconRenderer.color = color;
        }
    }
    
    public void SetBlinkingEnabled(bool enabled)
    {
        enableBlinking = enabled;
        
        if (isPlayerHere)
        {
            if (enabled)
            {
                StartBlinking();
            }
            else
            {
                StopBlinking();
            }
        }
    }
    
    public void SetBlinkSpeed(float speed)
    {
        blinkSpeed = Mathf.Max(0.1f, speed);
        
        // Reiniciar parpadeo con nueva velocidad si está activo
        if (isPlayerHere && enableBlinking)
        {
            StartBlinking();
        }
    }
    
    // Cleanup al destruir
    private void OnDestroy()
    {
        StopBlinking();
    }
    
    // NEW: Método público para saber si el jugador está aquí
    public bool IsPlayerHere()
    {
        return isPlayerHere;
    }
    
    // NEW: Método público para verificar si el icono está activo
    public bool IsPlayerIconActive()
    {
        return playerIcon != null && playerIcon.activeInHierarchy;
    }
    
    // NUEVO: Método para actualizar configuración desde la zona dinámicamente
    public void UpdateFromZoneConfig()
    {
        if (parentZone != null)
        {
            playerIconColor = parentZone.playerIconColor;
            enableBlinking = parentZone.enablePlayerIconBlinking;
            blinkSpeed = parentZone.playerIconBlinkSpeed;
        
            // Si el jugador está aquí, aplicar inmediatamente
            if (isPlayerHere)
            {
                SetPlayerPresence(true); // Refrescar con nueva configuración
            }
        }
    }
}