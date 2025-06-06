using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public PauseMenu PauseMenuScript { get; private set; }
    public SettingsMenu SettingsMenuScript { get; private set; }
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
    }
}
