using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FullMapController : MonoBehaviour
{
    [SerializeField] private Camera fullMapCamera;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 50f; // Aumentado para mejor sensación
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 25f;
    
    private Vector2 moveInput;
    private bool isMapOpen = false;

    // Suscribirse a los eventos cuando se activa el componente

    //private void Start()
    //{
    //    MenuInput.OnOpenMapButtonPressed += ToggleMap;
    //}

    private void OnEnable()
    {
        MenuInput.OnMapZoom += HandleZoom;
        MenuInput.OnMapNavigate += HandleNavigate;
    }

    // Desuscribirse cuando se desactiva
    private void OnDisable()
    {
        MenuInput.OnMapNavigate -= HandleNavigate;
        MenuInput.OnMapZoom -= HandleZoom;
    }

    //private void OnDestroy()
    //{
    //    MenuInput.OnOpenMapButtonPressed -= ToggleMap;
    //}

    public void ToggleMap()
    {
        // Solo podemos abrir el mapa si estamos jugando (no en menú o ya pausado)
        if (GameManager.Instance.CurrentGameState != GameState.Gameplay && !isMapOpen)
        {
            return;
        }

        isMapOpen = !isMapOpen;
        gameObject.SetActive(isMapOpen);

        if (isMapOpen)
        {
            OpenMap();
        }
        else
        {
            CloseMap();
        }
    }

    // Este método se llamaría desde el botón "Mapa" del menú de pausa
    public void OpenMap()
    {
        // Pausamos el juego para que no nos ataquen mientras vemos el mapa
        Time.timeScale = 0f;
        
        // Cambiamos al Action Map de UI para controlar el mapa
        var playerInput = GameManager.Instance.playerObject.GetComponent<PlayerInput>();
        if (playerInput != null) 
            playerInput.SwitchCurrentActionMap("OnMenu");

        // CAMBIO CLAVE: Centrar la cámara del mapa grande en la posición actual del jugador
        if (MapRoomManager.Instance != null && fullMapCamera != null)
        {
            MapRoomManager.Instance.CenterCameraOnPlayer(fullMapCamera);
        }
    }

    public void CloseMap()
    {
        // Reanudamos el juego
        Time.timeScale = 1f;
        
        // Volvemos al Action Map de juego
        var playerInput = GameManager.Instance.playerObject.GetComponent<PlayerInput>();
        if (playerInput != null) 
            playerInput.SwitchCurrentActionMap("OnGame");
    }

    // Manejadores de eventos
    private void HandleNavigate(Vector2 input)
    {
        if (!isMapOpen) return;
        moveInput = input;
    }
    
    private void HandleZoom(Vector2 scroll)
    {
        if (!isMapOpen || fullMapCamera == null) return;
        
        // Para la rueda del ratón, el valor está en Y
        float scrollValue = scroll.y;
        
        // Normalizar el scroll (suele ser 120 o -120)
        scrollValue = Mathf.Sign(scrollValue);

        float currentSize = fullMapCamera.orthographicSize;
        currentSize -= scrollValue * zoomSpeed * Time.unscaledDeltaTime;
        fullMapCamera.orthographicSize = Mathf.Clamp(currentSize, minZoom, maxZoom);
    }

    private void Update()
    {
        if (isMapOpen && fullMapCamera != null && moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 moveDelta = new Vector3(moveInput.x, moveInput.y, 0);
            // Usamos tiempo no escalado porque el juego está pausado
            fullMapCamera.transform.position += moveDelta * moveSpeed * Time.unscaledDeltaTime;
        }
    }
}