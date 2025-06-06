using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class MovementInput : MonoBehaviour
{
    public static event Action<float> OnHorizontalInput;
    public static event Action OnJumpPressed;

    public void ReadHorizontalInput(InputAction.CallbackContext context)
    {
        OnHorizontalInput?.Invoke(context.ReadValue<Vector2>().x);
    }

    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpPressed?.Invoke();
        }
    }
}