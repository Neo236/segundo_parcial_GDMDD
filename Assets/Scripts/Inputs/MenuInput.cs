using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInput : MonoBehaviour
{
    public static event Action OnPauseButtonPressed;

    public void ReadPauseInput(InputAction.CallbackContext context)
    {
        OnPauseButtonPressed?.Invoke();
    }
    
}
