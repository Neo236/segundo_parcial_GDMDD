using System;
using System.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event Action OnTakeDamage;

    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float debugDamageAmount = 1000f;
    [SerializeField] private float delayBeforeGameOver = 2f;

    private bool _isDead = false;
    private float _currentHealth = 1000f;
    private float _maxHealth = 100f;

    public bool IsDead => _isDead;
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;


    [Header("Animations")]
    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning($"No Animator found on {gameObject.name}. Animations will not play.");
        }
    }
    public void TakeDamage(float damage)
    {
        if (_animator != null)
        {
            _animator.SetTrigger("GetDamage");
        }
   public void OnEnable()
    {
        MenuInput.OnDebugButtonPressedDamage += HandleDebugDamage;
    }

    public void OnDisable()
    {
        MenuInput.OnDebugButtonPressedDamage -= HandleDebugDamage;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        AudioManager.Instance.PlaySfx(hurtSound);
        _currentHealth = CurrentHealth - damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
        OnTakeDamage?.Invoke();
    }

    public void Heal(float health)
    {
        if (_isDead) return;
        _currentHealth = Math.Clamp(CurrentHealth + health, 0f, _maxHealth);
        OnTakeDamage?.Invoke();
    }

    public void Die()
    {
        // Evitar que la lógica de muerte se ejecute múltiples veces si se recibe daño rápido
        if (_isDead) return;
        _isDead = true;

        Debug.Log("El jugador ha muerto.");

        // En lugar de llamar directamente al GameManager, iniciamos la corrutina de muerte.
        StartCoroutine(DeathSequence());
    }
    private IEnumerator DeathSequence()
    {
        // 1. Reproducir el sonido de la muerte
        AudioManager.Instance.PlaySfx(deathSound);

        // 2. (OPCIONAL PERO RECOMENDADO) Desactivar los controles del jugador
        //GetComponent<MovementInput>()?.enabled = false;
        //GetComponent<AttackInput>()?.enabled = false;
        // También podrías desactivar el Rigidbody2D para que se quede quieto
        // GetComponent<Rigidbody2D>().simulated = false;

        // 3. (OPCIONAL) Reproducir una animación de muerte
        // Animator anim = GetComponent<Animator>();
        // anim.SetTrigger("Die");

        // 4. ESPERAR el tiempo que definimos
        yield return new WaitForSeconds(delayBeforeGameOver);

        // 5. AHORA SÍ, le decimos al GameManager que inicie la transición
        GameManager.Instance.TriggerGameOver();
    }

    private void HandleDebugDamage()
    {
        // Solo podemos recibir daño si estamos en modo de juego
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            Debug.Log($"DEBUG: Aplicando {debugDamageAmount} de daño al jugador.");
            TakeDamage(debugDamageAmount);
        }
    }
    
    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
    }
}
