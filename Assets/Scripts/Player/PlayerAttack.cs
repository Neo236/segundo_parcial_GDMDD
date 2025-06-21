using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    [FormerlySerializedAs("attackSpwawner")]
    [Header("Attack Settings")]
    [SerializeField] private Transform attackSpawnerRight;
    [SerializeField] private Transform attackSpawnerLeft;
    [SerializeField] private Transform attackSpawnerUp;
    [SerializeField] private Transform attackSpawnerDown;


    [SerializeField] private GameObject attackProjectile;
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private AudioClip noInkSound; // Optional sound for when there's not enough ink

    private float _currentCooldownTimer = 0.2f;
    private PlayerInk _playerInk;
    private AttackSelector _attackSelector;
    private InkSelector _inkSelector;

    //private PlayerInput _playerInput;
    //private InputAction _moveAction;
    //private InputAction _attackAction;
    
    private PlayerMovement _playerMovement; 
    
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _attackAction;


    private Vector2 _lookDirection = Vector2.right;

    [Header("Animations")]
    private Animator _animator;
    private GameObject _playerSprite;
    private void Awake()
    {
        InitializeComponents();
        //_playerInput = GetComponent<PlayerInput>();
        //var actionMap = _playerInput.actions.FindActionMap("OnGame");
        //_moveAction = actionMap.FindAction("Move");
        //_attackAction = actionMap.FindAction("Attack");

        //_attackAction.performed += ctx => HandleAttack();
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

        if (_playerInk == null || _attackSelector == null || _inkSelector == null || _playerMovement == null)
        {
            Debug.LogError($"Missing required components on {gameObject.name}");
            enabled = false;
        }
    }

    private void Update()
    {
        _currentCooldownTimer += Time.deltaTime;

         // Lee el valor de Move para actualizar la dirección de mirada
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        _playerSprite = transform.Find("PlayerSprite")?.gameObject;
        _animator = _playerSprite?.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning($"No Animator found on {gameObject.name}. Animations will not play.");
        }

        // Prioridad: vertical sobre horizontal
        if (moveInput.y > 0.5f)
            _lookDirection = Vector2.up;
        else if (moveInput.y < -0.5f)
            _lookDirection = Vector2.down;
        else if (moveInput.x > 0.5f)
            _lookDirection = Vector2.right;
        else if (moveInput.x < -0.5f)
            _lookDirection = Vector2.left;
    }

    private void HandleAttack()
    {
        if (_currentCooldownTimer <= attackCooldown) return;

        if (_playerInk.CurrentInk < _attackSelector.selectedAttack.inkCost)
        {
            Debug.Log("Not enough ink to attack");
            if (noInkSound != null)
            {
                AudioManager.Instance.PlaySfx(noInkSound);
            }
            return;
        }
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
        PerformAttack();
    }

    private void TriggerAttack()
    { 
        PerformAttack();
    }
    private void PerformAttack()
    {

        _currentCooldownTimer = 0;
        _playerInk.CurrentInk -= _attackSelector.selectedAttack.inkCost;

        _attackSelector.selectedAttack.elementType = _inkSelector.currentInk;

        if (_attackSelector.selectedAttack.soundEffect != null)
        {
            AudioManager.Instance.PlaySfx(_attackSelector.selectedAttack.soundEffect);
        }

        Transform spawner;
        Vector3 direction;

        Vector2 lookDirection = _playerMovement.LookDirection;

        if (lookDirection == Vector2.right)
        {
            spawner = attackSpawnerRight;
            direction = Vector3.right;
        }
        else if (lookDirection == Vector2.left)
        {
            spawner = attackSpawnerLeft;
            direction = Vector3.left;
        }
        else if (lookDirection == Vector2.up)
        {
            spawner = attackSpawnerUp;
            direction = Vector3.up;
        }
        else // down
        {
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