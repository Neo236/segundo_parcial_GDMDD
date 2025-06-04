using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float fallGravityMultiplier = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    private float _initialGravityScale;
    private float _horizontalMovement;
    private float _timeSinceLastGroundTouch;
    private float _jumpBufferCounter;
    private bool _canJump;
    private Rigidbody2D _rb;
    private GroundCheck _groundCheck;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundCheck = GetComponent<GroundCheck>();

        if (_rb == null)
        {
            Debug.LogError($"No Rigidbody2D found on {gameObject.name}");
            enabled = false;
            return;
        }

        if (_groundCheck == null)
        {
            Debug.LogError($"No GroundCheck found on {gameObject.name}");
            enabled = false;
            return;
        }

        _initialGravityScale = _rb.gravityScale;
        
        // Subscribe to input events
        MovementInput.OnHorizontalInput += HandleHorizontalMovement;
        MovementInput.OnJumpPressed += HandleJumpInput;
    }

    private void OnDestroy()
    {
        MovementInput.OnHorizontalInput -= HandleHorizontalMovement; 
        MovementInput.OnJumpPressed -= HandleJumpInput;
    }

    private void Update()
    {
        if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Move();
        HandleJumpLogic();
        ApplyGravityModifiers();
    }

    private void Move()
    {
        _rb.linearVelocity = new Vector2(_horizontalMovement * speed, _rb.linearVelocity.y);
    }

    private void HandleHorizontalMovement(float movement)
    {
        _horizontalMovement = movement;
    }

    private void HandleJumpInput()
    {
        if (_canJump)
        {
            PerformJump();
        }
        else
        {
            _jumpBufferCounter = jumpBufferTime;
        }
    }

    private void HandleJumpLogic()
    {
        if (_groundCheck.IsGrounded)
        {
            if (_rb.linearVelocity.y <= 0.01f)
            {
                _timeSinceLastGroundTouch = 0f;
                _canJump = true;
            }
        }
        else
        {
            _timeSinceLastGroundTouch += Time.fixedDeltaTime;
            _canJump = _timeSinceLastGroundTouch < coyoteTime;
        }

        if (_canJump && _jumpBufferCounter > 0)
        {
            PerformJump();
            _jumpBufferCounter = 0;
        }
    }

    private void PerformJump()
    {
        AudioManager.Instance.PlaySfx(jumpSound);
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        _canJump = false;
        _timeSinceLastGroundTouch = coyoteTime + 1f;
    }

    private void ApplyGravityModifiers()
    {
        _rb.gravityScale = _rb.linearVelocity.y < 0 
            ? _initialGravityScale * fallGravityMultiplier 
            : _initialGravityScale;
    }
}