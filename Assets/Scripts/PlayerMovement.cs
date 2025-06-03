using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float _speed = 5f;
    private float _jumpForce = 5f;
    private float _isGrounded;

    private float m_coldDownTimer = 0.2f;

    [SerializeField] private Transform attackSpwawner;

    [SerializeField] private GameObject attackProjectile;

    float _horizontalMovement;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rb.linearVelocity = new Vector2(_horizontalMovement * _speed, _rb.linearVelocity.y);
    }


    private void FixedUpdate()
    {
        m_coldDownTimer += Time.deltaTime;

      
    }


    public void ReadFireInput(InputAction.CallbackContext context)
{
    if (context.performed && m_coldDownTimer > 0.2f)
    {
        Attack();
    }
}

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


    public void Attack()
    {
        m_coldDownTimer = 0;
        Debug.Log("Shoot");
        GameObject projectileInstance = Instantiate(attackProjectile, attackSpwawner.position, attackSpwawner.rotation);
        Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        projectileInstance.GetComponent<AttackProjectile>().Initialize(direction);
    }
}

