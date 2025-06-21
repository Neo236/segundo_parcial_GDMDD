using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInput : MonoBehaviour
{
    //public static event Action OnPauseButtonPressed;
    public static event Action OnBackButtonPressed;
    public static event Action OnDebugButtonPressedDamage;
    public static event Action OnDebugButtonPressedVictory;
    
    // ¡NUEVOS EVENTOS PARA EL MAPA!
    public static event Action OnOpenMapButtonPressed;
    public static event Action<Vector2> OnMapNavigate;
    public static event Action<Vector2> OnMapZoom;

    public void ReadPauseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.TogglePause();
        }
    }
    
    public void ReadBackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnBackButtonPressed?.Invoke();
        }
    }
    
    // ¡NUEVO MÉTODO PARA ABRIR/CERRAR EL MAPA!
    public void ReadOpenMapInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnOpenMapButtonPressed?.Invoke();
        }
    }
    
    // NUEVOS MÉTODOS PARA CONTROLAR EL MAPA
    public void ReadNavigateInput(InputAction.CallbackContext context)
    {
        OnMapNavigate?.Invoke(context.ReadValue<Vector2>());
    }

    public void ReadScrollInput(InputAction.CallbackContext context)
    {
        OnMapZoom?.Invoke(context.ReadValue<Vector2>());
    }

    public void ReadDebugInputTakeDamage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnDebugButtonPressedDamage?.Invoke();
        }
    }
    public void ReadDebugInputVictory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnDebugButtonPressedVictory?.Invoke();
        }
    }
}