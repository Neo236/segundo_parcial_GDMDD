using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform attackSpawnerRight;
    [SerializeField] private Transform attackSpawnerLeft;
    [SerializeField] private Transform attackSpawnerUp;
    [SerializeField] private Transform attackSpawnerDown;

    [SerializeField] private GameObject attackProjectile;
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private AudioClip noInkSound;

    private float _currentCooldownTimer = 0.2f;
    private PlayerInk _playerInk;
    private AttackSelector _attackSelector;
    private InkSelector _inkSelector;
    
    // La única referencia externa que necesitamos
    private PlayerMovement _playerMovement; 

    // ¡NUEVO! Referencia al Animator
    private Animator _animator;

    private void Awake()
    {
        InitializeComponents();
    }

    private void OnEnable()
    {
        AttackInput.OnAttackButtonPressed += HandleAttack;
    }

    private void OnDisable()
    {
        AttackInput.OnAttackButtonPressed -= HandleAttack;
    }

    private void InitializeComponents()
    {
        _playerInk = GetComponent<PlayerInk>();
        _attackSelector = GetComponent<AttackSelector>();
        _inkSelector = GetComponent<InkSelector>();
        _playerMovement = GetComponent<PlayerMovement>();
        
        // Obtenemos el Animator desde el PlayerSprite hijo
        var playerSprite = transform.Find("PlayerSprite");
        if (playerSprite != null)
        {
            _animator = playerSprite.GetComponent<Animator>();
        }

        if (_playerMovement == null || _playerInk == null || _attackSelector == null || _inkSelector == null)
        {
            Debug.LogError($"Faltan componentes requeridos en {gameObject.name}");
            enabled = false;
        }
    }

    // ¡EL MÉTODO UPDATE AHORA ES MUY SIMPLE!
    private void Update()
    {
        _currentCooldownTimer += Time.deltaTime;
    }

    private void HandleAttack()
    {
        if (_currentCooldownTimer <= attackCooldown) return;

        if (_playerInk.CurrentInk < _attackSelector.selectedAttack.inkCost)
        {
            Debug.Log("No hay suficiente tinta para atacar.");
            if (noInkSound != null)
            {
                AudioManager.Instance.PlaySfx(noInkSound);
            }
            return;
        }
        
        PerformAttack();
    }

    private void PerformAttack()
    {
        _currentCooldownTimer = 0;
        _playerInk.CurrentInk -= _attackSelector.selectedAttack.inkCost;
        _attackSelector.selectedAttack.elementType = _inkSelector.currentInk;

        // Disparamos la animación de ataque
        _animator?.SetTrigger("Attack");

        if (_attackSelector.selectedAttack.soundEffect != null)
        {
            AudioManager.Instance.PlaySfx(_attackSelector.selectedAttack.soundEffect);
        }

        Transform spawner;
        Vector3 direction;

        // ¡OBTENEMOS LA DIRECCIÓN DEL PLAYER MOVEMENT!
        Vector2 lookDirection = _playerMovement.LookDirection;

        if (lookDirection == Vector2.right) {
            spawner = attackSpawnerRight;
            direction = Vector3.right;
        } else if (lookDirection == Vector2.left) {
            spawner = attackSpawnerLeft;
            direction = Vector3.left;
        } else if (lookDirection == Vector2.up) {
            spawner = attackSpawnerUp;
            direction = Vector3.up;
        } else { // down
            spawner = attackSpawnerDown;
            direction = Vector3.down;
        }

        GameObject projectileInstance = Instantiate(attackProjectile, spawner.position, spawner.rotation);
        projectileInstance.GetComponent<AttackProjectile>().Initialize(direction, _attackSelector.selectedAttack);

        LogAttackDetails();
    }

    private void LogAttackDetails()
    {
        Debug.Log($"Selected Attack: {_attackSelector.selectedAttack.name} " +
                  $"with ink cost: {_attackSelector.selectedAttack.inkCost} " +
                  $"and element type: {_attackSelector.selectedAttack.elementType}");
    }
}