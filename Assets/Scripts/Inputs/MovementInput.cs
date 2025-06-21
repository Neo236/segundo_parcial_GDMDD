using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class MovementInput : MonoBehaviour
{
    public static event Action<Vector2> OnMoveInput;
    public static event Action OnJumpPressed;
    public static event Action OnJumpReleased; // ¡NUEVO EVENTO!

    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            // Leemos el Vector2 completo y lo emitimos.
            OnMoveInput?.Invoke(context.ReadValue<Vector2>());
        }
        else
        {
            // Si no estamos jugando, aseguramos enviar un vector cero para detener el movimiento.
            OnMoveInput?.Invoke(Vector2.zero);
        }
    }
    
    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        // Si no estamos jugando, no hacemos nada.
        if (GameManager.Instance.CurrentGameState != GameState.Gameplay) return;
        
        if (context.performed) // Se ejecuta cuando se presiona el botón
        {
            OnJumpPressed?.Invoke();
        }
        else if (context.canceled) // Se ejecuta cuando se suelta el botón
        {
            OnJumpReleased?.Invoke();
        }
    }
}