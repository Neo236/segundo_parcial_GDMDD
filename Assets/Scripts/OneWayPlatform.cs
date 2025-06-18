using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OneWayPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private LayerMask playerLayerMask = -1;
    [SerializeField] private bool useGroundCheckIntegration = true;
    [SerializeField] private float velocityThreshold = -1f;
    [SerializeField] private float supportHeight = 1.5f; // MÁS ALTURA
    [SerializeField] private float exitThreshold = 1.0f; // Nueva: distancia para salir
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private Collider2D platformCollider;
    private PlayerMovement currentPlayer;
    private bool isPlayerSupported = false;
    private float platformTopY;
    
    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
        platformCollider.isTrigger = true;
        platformTopY = platformCollider.bounds.max.y;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return;
        
        // NO soportar si está en modo dropping
        if (player.IsDropping)
        {
            if (showDebugInfo)
                Debug.Log($"🚫 Jugador en dropping mode - NO soportar");
            return;
        }
        
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        
        // DETECCIÓN INMEDIATA en OnTriggerEnter2D
        bool isFallingFast = rb.linearVelocity.y <= velocityThreshold;
        float playerBottom = other.bounds.min.y;
        float distanceToTop = playerBottom - platformTopY;
        bool isNearTop = distanceToTop <= supportHeight && distanceToTop >= -0.2f; // Más tolerancia
        
        if (showDebugInfo)
        {
            Debug.Log($"🔵 JUGADOR ENTRA AL TRIGGER");
            Debug.Log($"   Posición jugador: {other.transform.position.y:F2}");
            Debug.Log($"   Top plataforma: {platformTopY:F2}");
            Debug.Log($"   Velocidad Y: {rb.linearVelocity.y:F2}");
            Debug.Log($"   Distancia al top: {distanceToTop:F2}");
            Debug.Log($"   ¿Cayendo rápido?: {isFallingFast}");
            Debug.Log($"   ¿Cerca del top?: {isNearTop}");
        }
        
        // ACTIVAR SOPORTE INMEDIATAMENTE si cumple condiciones
        if (isFallingFast && isNearTop)
        {
            currentPlayer = player;
            StartSupportingPlayer(other);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return;
        
        // Si está en dropping mode, desactivar soporte
        if (player.IsDropping && isPlayerSupported)
        {
            StopSupportingPlayer("Jugador activó dropping mode");
            return;
        }
        
        // MANTENER SOPORTE: No hacer nada si ya está siendo soportado
        if (isPlayerSupported) return;
        
        // Lógica de backup por si OnTriggerEnter2D falló
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        
        bool isFallingFast = rb.linearVelocity.y <= velocityThreshold;
        float playerBottom = other.bounds.min.y;
        float distanceToTop = playerBottom - platformTopY;
        bool isNearTop = distanceToTop <= supportHeight && distanceToTop >= -0.2f;
        
        if (isFallingFast && isNearTop)
        {
            currentPlayer = player;
            StartSupportingPlayer(other);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        
        // NUEVA CONDICIÓN: Solo desactivar si está REALMENTE lejos
        if (isPlayerSupported)
        {
            float playerY = other.transform.position.y;
            float distanceFromTop = playerY - platformTopY;
            
            if (showDebugInfo)
            {
                Debug.Log($"🔍 EVALUANDO SALIDA DEL TRIGGER");
                Debug.Log($"   Jugador Y: {playerY:F2}");
                Debug.Log($"   Plataforma Top: {platformTopY:F2}");
                Debug.Log($"   Distancia desde top: {distanceFromTop:F2}");
                Debug.Log($"   Umbral de salida: {exitThreshold:F2}");
            }
            
            // Solo desactivar si está MUY lejos (abajo o muy arriba)
            bool isTooFarBelow = distanceFromTop < -exitThreshold;
            bool isTooFarAbove = distanceFromTop > exitThreshold + 1.0f;
            
            if (isTooFarBelow || isTooFarAbove)
            {
                string reason = isTooFarBelow ? "Jugador muy abajo de la plataforma" : "Jugador muy arriba de la plataforma";
                StopSupportingPlayer(reason);
            }
            else
            {
                if (showDebugInfo)
                    Debug.Log($"🔒 MANTENIENDO SOPORTE - Jugador aún cerca de la plataforma");
                return; // NO desactivar el soporte
            }
        }
        
        currentPlayer = null;
        
        if (showDebugInfo)
        {
            Debug.Log($"🔴 JUGADOR SALE DEL TRIGGER");
        }
    }
    
    private bool IsPlayer(Collider2D collider)
    {
        return collider.CompareTag("Player") || 
               ((1 << collider.gameObject.layer) & playerLayerMask) != 0;
    }
    
    private void StartSupportingPlayer(Collider2D playerCollider)
    {
        isPlayerSupported = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"⬆️ ¡SOPORTE ACTIVADO! - Tiempo: {Time.time:F2}");
            Debug.Log($"⬆️ Jugador: {playerCollider.name} en Y: {playerCollider.transform.position.y:F2}");
        }
        
        // PARAR CAÍDA INMEDIATAMENTE
        Rigidbody2D rb = playerCollider.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // STOP total de la velocidad vertical
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            
            // Posicionar al jugador exactamente en el top de la plataforma
            float playerHalfHeight = playerCollider.bounds.size.y * 0.5f;
            float targetY = platformTopY + playerHalfHeight + 0.05f; // Offset más generoso
            
            Vector3 newPos = playerCollider.transform.position;
            newPos.y = targetY;
            playerCollider.transform.position = newPos;
            
            if (showDebugInfo)
            {
                Debug.Log($"⬆️ Velocidad ajustada a: {rb.linearVelocity}");
                Debug.Log($"⬆️ Posición ajustada a Y: {newPos.y:F2}");
            }
        }
        
        // Notificar a GroundCheck
        if (useGroundCheckIntegration)
        {
            GroundCheck groundCheck = playerCollider.GetComponent<GroundCheck>();
            if (groundCheck != null)
            {
                groundCheck.ForceGroundedState(true, transform);
                if (showDebugInfo)
                    Debug.Log($"🔗 GroundCheck notificado: GROUNDED");
            }
        }
    }
    
    private void StopSupportingPlayer(string reason = "")
    {
        if (showDebugInfo)
        {
            Debug.Log($"⬇️ SOPORTE DESACTIVADO - Tiempo: {Time.time:F2}");
            Debug.Log($"⬇️ Razón: {reason}");
        }
        
        isPlayerSupported = false;
        
        // Notificar a GroundCheck
        if (useGroundCheckIntegration && currentPlayer != null)
        {
            GroundCheck groundCheck = currentPlayer.GetComponent<GroundCheck>();
            if (groundCheck != null)
            {
                groundCheck.ClearForcedState();
                if (showDebugInfo)
                    Debug.Log($"🔗 GroundCheck liberado");
            }
        }
    }
    
    // API pública
    public bool IsPlayerSupported => isPlayerSupported;
    
    private void OnDrawGizmos()
    {
        if (!showDebugInfo) return;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            // Plataforma
            Gizmos.color = isPlayerSupported ? Color.green : Color.yellow;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            
            // Línea del top de la plataforma (ROJA y GRUESA)
            Gizmos.color = Color.red;
            Vector3 lineStart = new Vector3(col.bounds.min.x, col.bounds.max.y, 0);
            Vector3 lineEnd = new Vector3(col.bounds.max.x, col.bounds.max.y, 0);
            Gizmos.DrawLine(lineStart, lineEnd);
            
            // ZONA DE SOPORTE (encima de la plataforma)
            Gizmos.color = Color.cyan;
            float supportTop = col.bounds.max.y + supportHeight;
            Vector3 supportLineStart = new Vector3(col.bounds.min.x, supportTop, 0);
            Vector3 supportLineEnd = new Vector3(col.bounds.max.x, supportTop, 0);
            Gizmos.DrawLine(supportLineStart, supportLineEnd);
            
            // ZONA DE SALIDA (umbral para desactivar soporte)
            Gizmos.color = Color.magenta;
            float exitLineTop = col.bounds.max.y + exitThreshold + 1.0f;
            float exitLineBottom = col.bounds.max.y - exitThreshold;
            
            Vector3 exitTopStart = new Vector3(col.bounds.min.x, exitLineTop, 0);
            Vector3 exitTopEnd = new Vector3(col.bounds.max.x, exitLineTop, 0);
            Gizmos.DrawLine(exitTopStart, exitTopEnd);
            
            Vector3 exitBottomStart = new Vector3(col.bounds.min.x, exitLineBottom, 0);
            Vector3 exitBottomEnd = new Vector3(col.bounds.max.x, exitLineBottom, 0);
            Gizmos.DrawLine(exitBottomStart, exitBottomEnd);
            
            // Rectángulo de la zona de soporte
            Gizmos.color = new Color(0, 1, 1, 0.2f); // Cyan semi-transparente
            Vector3 supportCenter = new Vector3(col.bounds.center.x, col.bounds.max.y + supportHeight/2, 0);
            Vector3 supportSize = new Vector3(col.bounds.size.x, supportHeight, 0);
            Gizmos.DrawCube(supportCenter, supportSize);
        }
    }
}