// GameDebugger.cs
using UnityEngine;

public class GameDebugger : MonoBehaviour
{
    [SerializeField] private float debugDamageAmount = 50f;

    private void OnEnable()
    {
        MenuInput.OnDebugButtonPressedDamage += HandleDebugDamage;
        MenuInput.OnDebugButtonPressedVictory += HandleDebugVictory;
    }

    private void OnDisable()
    {
        MenuInput.OnDebugButtonPressedDamage -= HandleDebugDamage;
        MenuInput.OnDebugButtonPressedVictory -= HandleDebugVictory;
    }

    private void HandleDebugDamage()
    {
        // Solo si estamos jugando
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            Debug.Log($"DEBUG: Aplicando {debugDamageAmount} de daño al jugador.");
            GameManager.Instance.playerObject.GetComponent<PlayerHealth>()?.TakeDamage(debugDamageAmount);
        }
    }

    private void HandleDebugVictory()
    {
        // Solo si estamos jugando
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            Debug.Log("DEBUG: Forzando la escena de victoria.");
            GameManager.Instance.TriggerEndScene();
        }
    }
}