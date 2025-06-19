using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public PauseMenu PauseMenuScript { get; private set; }
    public SettingsMenu SettingsMenuScript { get; private set; }

    public bool forPhone = false;
    public GameObject mapCanvas;
    public FullMapController fullMapController;
    public GameObject onScreenInputs;
    // Puedes añadir más scripts de UI aquí si los necesitas (ej: HUDController)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PauseMenuScript = GetComponent<PauseMenu>();
        if (PauseMenuScript == null) Debug.LogError("UIManager no pudo encontrar el componente PauseMenu.");
        
        SettingsMenuScript = GetComponent<SettingsMenu>();
        if (SettingsMenuScript == null) Debug.LogError("UIManager no pudo encontrar el componente SettingsMenu.");
        
        fullMapController = mapCanvas.GetComponentInChildren<FullMapController>(true);

        onScreenInputs.SetActive(forPhone);
    }
    private void OnEnable()
    {
        MenuInput.OnOpenMapButtonPressed += HandleOpenMapToggle;
    }

    private void OnDisable()
    {
        MenuInput.OnOpenMapButtonPressed -= HandleOpenMapToggle;
    }

    private void HandleOpenMapToggle()
    {
        if (fullMapController != null)
        {
            // Le pasamos la orden al controlador del mapa
            fullMapController.ToggleMap();
        }
    }

}
