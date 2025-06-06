using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInput : MonoBehaviour
{
    public static event Action OnPauseButtonPressed;
    public static event Action OnBackButtonPressed;

    public void ReadPauseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPauseButtonPressed?.Invoke();
        }
    }
    
    public void ReadBackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnBackButtonPressed?.Invoke();
        }
    }
    
}
