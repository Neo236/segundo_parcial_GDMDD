using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float _speed = 5f;
    private float _jumpForce = 5f;
    private float _isGrounded;

    private PlayerHealthj _playerHealth;

    private AttackSelector _attackSelector;

    private AttackData _selectedAttack;

    private float m_coldDownTimer = 0.2f;

    [SerializeField] private Transform attackSpwawner;

    [SerializeField] private GameObject attackProjectile;

    float _horizontalMovement;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerHealth = GetComponent<PlayerHealthj>();
        _attackSelector = GetComponent<AttackSelector>();
        if (_attackSelector != null)
        {
            _selectedAttack = _attackSelector.selectedAttack;
        }
        else
        {
            Debug.LogWarning("AttackSelector component not found on PlayerMovement.");
        }
    }

    private void Update()
    {
        m_coldDownTimer += Time.deltaTime;
        _rb.linearVelocity = new Vector2(_horizontalMovement * _speed, _rb.linearVelocity.y);
    }





    /*public void ReadFireInput(InputAction.CallbackContext context)
    {
        Debug.Log("ReadFireInput triggered: " + context.phase);
        if (context.performed && m_coldDownTimer > 0.2f)
        {
            Attack();
        }
    }*/


    public void ReadHorizontalInput(InputAction.CallbackContext context)
    {
        _horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Jump();
        }
    }

    public void Jump()
    {
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }


    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && m_coldDownTimer > 0.2f)
        {
            if (_playerHealth.currentInk >= _selectedAttack.inkCost)
            {
                m_coldDownTimer = 0;
                _playerHealth.currentInk -= _selectedAttack.inkCost;
               
                GameObject projectileInstance = Instantiate(attackProjectile, attackSpwawner.position, attackSpwawner.rotation);
                Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
                projectileInstance.GetComponent<AttackProjectile>().Initialize(direction, _selectedAttack);
            }
            else
            {
                Debug.Log("Not enough ink to attack");
            }
        }
    }
    


}

