using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private FullMapController fullMapController;
    
    [Header("Mobile Controls")]
    [SerializeField] private GameObject onScreenControls;
    [SerializeField] private bool enableOnScreenControls = false;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    // Propiedades públicas para acceso desde otros scripts
    public PauseMenu PauseMenuScript => pauseMenu;
    public SettingsMenu SettingsMenuScript => settingsMenu;
    public FullMapController FullMapControllerScript => fullMapController;
    
    // --- LÓGICA CENTRALIZADA DE SELECCIÓN UI ---
    private GameObject _lastSelectedButton;
    private bool _inputManagerSubscribed = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // ❌ COMENTAR TEMPORALMENTE: DontDestroyOnLoad(gameObject);
            
            if (debugMode) Debug.Log("UIManager: Inicializando...");
            
            // Buscar componentes si no están asignados
            FindUIComponents();
            
            // Configurar controles en pantalla según el booleano
            if (onScreenControls != null)
            {
                onScreenControls.SetActive(enableOnScreenControls);
                if (debugMode) Debug.Log($"UIManager: OnScreenControls configurado a {enableOnScreenControls}");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Intentar suscribirse aquí también por si InputDeviceManager se inicializa en Start
        TrySubscribeToInputManager();
    }

    private void FindUIComponents()
    {
        if (pauseMenu == null)
        {
            pauseMenu = GetComponentInChildren<PauseMenu>(true);
            if (debugMode) Debug.Log($"UIManager: PauseMenu encontrado: {(pauseMenu != null ? "SÍ" : "NO")}");
        }
        
        if (settingsMenu == null)
        {
            settingsMenu = GetComponentInChildren<SettingsMenu>(true);
            if (debugMode) Debug.Log($"UIManager: SettingsMenu encontrado: {(settingsMenu != null ? "SÍ" : "NO")}");
        }
        
        if (fullMapController == null)
        {
            fullMapController = GetComponentInChildren<FullMapController>(true);
            if (debugMode) Debug.Log($"UIManager: FullMapController encontrado: {(fullMapController != null ? "SÍ" : "NO")}");
        }
    }

    private void TrySubscribeToInputManager()
    {
        if (!_inputManagerSubscribed && InputDeviceManager.Instance != null)
        {
            InputDeviceManager.OnInputDeviceChanged += HandleInputDeviceChange;
            _inputManagerSubscribed = true;
            if (debugMode) Debug.Log("UIManager: ✅ Suscrito a InputDeviceManager.OnInputDeviceChanged");
        }
        else if (!_inputManagerSubscribed)
        {
            if (debugMode) Debug.Log("UIManager: InputDeviceManager aún no disponible, reintentando...");
            // Reintentar en el siguiente frame
            StartCoroutine(RetrySubscription());
        }
    }

    private IEnumerator RetrySubscription()
    {
        yield return new WaitForEndOfFrame();
        TrySubscribeToInputManager();
    }

    private void OnEnable()
    {
        if (debugMode) Debug.Log("UIManager: OnEnable - Suscribiéndose a eventos...");
        
        // Intentar suscribirse
        TrySubscribeToInputManager();
        
        MenuInput.OnOpenMapButtonPressed += HandleOpenMapToggle;
    }

    private void OnDisable()
    {
        if (debugMode) Debug.Log("UIManager: OnDisable - Desuscribiéndose de eventos...");
        
        if (_inputManagerSubscribed && InputDeviceManager.Instance != null)
        {
            InputDeviceManager.OnInputDeviceChanged -= HandleInputDeviceChange;
            _inputManagerSubscribed = false;
        }
        
        MenuInput.OnOpenMapButtonPressed -= HandleOpenMapToggle;
    }

    private void HandleOpenMapToggle()
    {
        if (fullMapController != null)
        {
            fullMapController.ToggleMap();
        }
    }

    // --- LÓGICA DEL SISTEMA DE SELECCIÓN UI CENTRALIZADO ---

    private void HandleInputDeviceChange(LastInputDevice deviceType)
    {
        if (debugMode) Debug.Log($"UIManager: ✅ InputDeviceChange detectado: {deviceType}");
        
        if (deviceType == LastInputDevice.KeyboardAndGamepad)
        {
            if (debugMode) Debug.Log($"UIManager: Cambiando a Keyboard/Gamepad. Último botón: {(_lastSelectedButton != null ? _lastSelectedButton.name : "NULL")}");
            
            // Al cambiar a teclado/gamepad, restaurar último botón seleccionado
            if (_lastSelectedButton != null && _lastSelectedButton.activeInHierarchy)
            {
                SetSelectedButton(_lastSelectedButton);
            }
            else if (_lastSelectedButton != null && !_lastSelectedButton.activeInHierarchy)
            {
                if (debugMode) Debug.LogWarning($"UIManager: El último botón '{_lastSelectedButton.name}' no está activo!");
            }
        }
        else if (deviceType == LastInputDevice.Mouse)
        {
            if (debugMode) Debug.Log("UIManager: Cambiando a Mouse. Deseleccionando todo.");
            // Al cambiar a ratón, deseleccionar todo
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void Update()
    {
        // Guardar el último botón seleccionado (solo con teclado/gamepad)
        if (InputDeviceManager.Instance != null && 
            InputDeviceManager.Instance.LastUsedDevice != LastInputDevice.Mouse)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                if (EventSystem.current.currentSelectedGameObject != _lastSelectedButton)
                {
                    _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
                    if (debugMode) Debug.Log($"UIManager: Nuevo botón guardado: {_lastSelectedButton.name}");
                }
            }
        }
    }

    /// <summary>
    /// Método público para que otros scripts soliciten la selección de un botón específico.
    /// </summary>
    public void SetSelectedButton(GameObject button)
    {
        if (debugMode) Debug.Log($"UIManager: SetSelectedButton llamado con: {(button != null ? button.name : "NULL")}");
        
        if (button != null && button.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(button);
            _lastSelectedButton = button;
            if (debugMode) Debug.Log($"UIManager: Botón '{button.name}' seleccionado y guardado.");
        }
        else if (button == null)
        {
            // Permitir deseleccionar explícitamente
            EventSystem.current.SetSelectedGameObject(null);
            if (debugMode) Debug.Log("UIManager: Selección limpiada.");
        }
        else if (!button.activeInHierarchy)
        {
            if (debugMode) Debug.LogWarning($"UIManager: No se puede seleccionar '{button.name}' porque no está activo!");
        }
    }

    /// <summary>
    /// Método de conveniencia para obtener el último botón seleccionado
    /// </summary>
    public GameObject GetLastSelectedButton()
    {
        return _lastSelectedButton;
    }

    // Método de debug para verificar el estado
    [ContextMenu("Debug UIManager State")]
    public void DebugState()
    {
        Debug.Log($"=== UIManager Debug State ===");
        Debug.Log($"InputDeviceManager.Instance: {(InputDeviceManager.Instance != null ? "EXISTS" : "NULL")}");
        if (InputDeviceManager.Instance != null)
        {
            Debug.Log($"Last Used Device: {InputDeviceManager.Instance.LastUsedDevice}");
        }
        Debug.Log($"EventSystem Current Selected: {(EventSystem.current.currentSelectedGameObject != null ? EventSystem.current.currentSelectedGameObject.name : "NULL")}");
        Debug.Log($"Last Selected Button: {(_lastSelectedButton != null ? _lastSelectedButton.name : "NULL")}");
        Debug.Log($"Input Manager Subscribed: {_inputManagerSubscribed}");
        Debug.Log($"========================");
    }
}