using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    [FormerlySerializedAs("attackSpwawner")]
    [Header("Attack Settings")]
    [SerializeField] private Transform attackSpawner;
    [SerializeField] private GameObject attackProjectile;
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private AudioClip noInkSound; // Optional sound for when there's not enough ink

    private float _currentCooldownTimer = 0.2f;
    private PlayerInk _playerInk;
    private AttackSelector _attackSelector;
    private InkSelector _inkSelector;

    private void Awake()
    {
        InitializeComponents();
        AttackInput.OnAttackButtonPressed += HandleAttack;
    }

    private void OnDestroy()
    {
        AttackInput.OnAttackButtonPressed -= HandleAttack;
    }

    private void InitializeComponents()
    {
        _playerInk = GetComponent<PlayerInk>();
        _attackSelector = GetComponent<AttackSelector>();
        _inkSelector = GetComponent<InkSelector>();

        if (_playerInk == null || _attackSelector == null || _inkSelector == null)
        {
            Debug.LogError($"Missing required components on {gameObject.name}");
            enabled = false;
        }
    }

    private void Update()
    {
        _currentCooldownTimer += Time.deltaTime;
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

        PerformAttack();
    }
    
    private void PerformAttack()
    {
        _currentCooldownTimer = 0;
        _playerInk.CurrentInk -= _attackSelector.selectedAttack.inkCost;
        
        _attackSelector.selectedAttack.elementType = _inkSelector.currentInk;
        
        // Play the attack sound if one is assigned
        if (_attackSelector.selectedAttack.soundEffect != null)
        {
            AudioManager.Instance.PlaySfx(_attackSelector.selectedAttack.soundEffect);
        }
        
        GameObject projectileInstance = Instantiate(attackProjectile, attackSpawner.position, attackSpawner.rotation);
        Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
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