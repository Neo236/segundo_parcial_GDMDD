using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class MovementInput : MonoBehaviour
{
    public static event Action<float> OnHorizontalInput;
    public static event Action<float> OnVerticalInput; // NUEVO
    public static event Action OnJumpPressed;

    public void ReadHorizontalInput(InputAction.CallbackContext context)
    {
        OnHorizontalInput?.Invoke(context.ReadValue<Vector2>().x);
    }
    
    // NUEVO: Leer input vertical
    public void ReadVerticalInput(InputAction.CallbackContext context)
    {
        OnVerticalInput?.Invoke(context.ReadValue<Vector2>().y);
    }
    
    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpPressed?.Invoke();
        }
    }
}