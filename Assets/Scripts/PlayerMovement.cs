using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float _speed = 5f;
    private float _jumpForce = 5f;
    private float _isGrounded;
    
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
}

