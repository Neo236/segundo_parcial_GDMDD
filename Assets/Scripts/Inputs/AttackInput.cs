using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackInput : MonoBehaviour
{
    public static event Action OnAttackButtonPressed;
    public static event Action OnSwitchAttackButtonPressed;
    public static event Action OnSwitchInkButtonPressed;

    public void ReadAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAttackButtonPressed?.Invoke();
        }
    }

    public void ReadSwitchAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSwitchAttackButtonPressed?.Invoke();
        }
    }

    public void ReadSwitchInkInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSwitchInkButtonPressed?.Invoke();
        }
    }
}
