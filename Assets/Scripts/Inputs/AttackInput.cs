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
        OnAttackButtonPressed?.Invoke();
    }

    public void ReadSwitchAttackInput(InputAction.CallbackContext context)
    {
        OnSwitchAttackButtonPressed?.Invoke();
    }

    public void ReadSwitchInkInput(InputAction.CallbackContext context)
    {
        OnSwitchInkButtonPressed?.Invoke();
    }
}
