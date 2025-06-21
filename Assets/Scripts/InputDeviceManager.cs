using UnityEngine;
using UnityEngine.InputSystem;

public enum LastInputDevice { None, Mouse, KeyboardAndGamepad }

public class InputDeviceManager : MonoBehaviour
{
    public static InputDeviceManager Instance { get; private set; }
    public LastInputDevice LastUsedDevice { get; private set; }

    public static event System.Action<LastInputDevice> OnInputDeviceChanged;

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

    private void OnEnable()
    {
        // Nos suscribimos al evento global de cualquier input
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        // Solo nos interesa cuando una acción se ejecuta
        if (change != InputActionChange.ActionPerformed) return;

        // Obtenemos el dispositivo que causó la acción
        var inputAction = obj as InputAction;
        if (inputAction == null || inputAction.activeControl == null) return;
        
        var activeDevice = inputAction.activeControl.device;

        // Determinamos el tipo de dispositivo
        LastInputDevice newDeviceType = LastUsedDevice;
        
        if (activeDevice is Mouse)
        {
            newDeviceType = LastInputDevice.Mouse;
        }
        else if (activeDevice is Keyboard || activeDevice is Gamepad)
        {
            newDeviceType = LastInputDevice.KeyboardAndGamepad;
        }

        // Si el tipo de dispositivo ha cambiado, disparamos un evento
        if (newDeviceType != LastUsedDevice)
        {
            LastUsedDevice = newDeviceType;
            Debug.Log($"Input device changed to: {LastUsedDevice}");
            OnInputDeviceChanged?.Invoke(LastUsedDevice);
        }
    }
}