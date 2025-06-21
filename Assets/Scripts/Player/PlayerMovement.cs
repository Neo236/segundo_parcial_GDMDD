using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float fallGravityMultiplier = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    
    [Header("Variable Jump Settings")]
    [SerializeField] private float jumpHoldDuration = 0.2f;
    [SerializeField] private float jumpHoldForce = 20f;
    
    [Header("Platform Settings")]
    [SerializeField] private float dropThroughDuration = 0.5f;
    [SerializeField] private float platformDetectionRadius = 0.6f;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    [Header("Animations")]

    private GameObject _playerSprite;
    private Animator _animator;

    private float _initialGravityScale;
    private float _horizontalMovement;
    private float _timeSinceLastGroundTouch;
    private float _jumpBufferCounter;
    private float _dropThroughTimer;
    private float _jumpHoldTimer;
    private bool _canJump;
    private bool _isHoldingJump = false;
    private Rigidbody2D _rb;
    private GroundCheck _groundCheck;
    
    // Propiedades públicas para las plataformas
    public bool IsDropping => _dropThroughTimer > 0f;
    public bool IsGrounded => _groundCheck.IsGrounded;
    public Vector2 LookDirection { get; private set; } = Vector2.right;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundCheck = GetComponent<GroundCheck>();
        _playerSprite = transform.Find("PlayerSprite")?.gameObject;
        _animator = _playerSprite.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning($"No Animator found on {gameObject.name}. Animations will not play.");
        }

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
    }

    private void OnEnable()
    {
        MovementInput.OnMoveInput += HandleMoveInput;
        MovementInput.OnJumpPressed += HandleJumpPressed;
        MovementInput.OnJumpReleased += HandleJumpReleased; // ¡Nos suscribimos al nuevo evento!
    }

    private void OnDisable()
    {
        MovementInput.OnMoveInput -= HandleMoveInput; 
        MovementInput.OnJumpPressed -= HandleJumpPressed;
        MovementInput.OnJumpReleased -= HandleJumpReleased;
    }

    private void Update()
    {
        if (_jumpBufferCounter > 0)
            _jumpBufferCounter -= Time.deltaTime;
        
        if (_dropThroughTimer > 0)
            _dropThroughTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Move();
        HandleJumpLogic();
        HandleJumpHold(); // Nueva lógica de sostenimiento
        ApplyGravityModifiers();
    }

    private void Move()
    {
        _rb.linearVelocity = new Vector2(_horizontalMovement * speed, _rb.linearVelocity.y);
        if (_animator != null)
    {
        _animator.SetBool("isWalking", Mathf.Abs(_horizontalMovement) > 0.01f);
    }
    }
    
    private void HandleMoveInput(Vector2 moveInput)
    {
        _horizontalMovement = moveInput.x;

        if (moveInput.sqrMagnitude > 0.1f)
        {
            if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
            {
                LookDirection = new Vector2(0, Mathf.Sign(moveInput.y));
            }
            else
            {
                LookDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
            }
        }
    }

    private void HandleJumpPressed()
    {
        if (_groundCheck.IsGrounded && LookDirection.y < -0.5f)
        {
            OneWayPlatform platform = GetCurrentOneWayPlatform();
            if (platform != null && platform.IsPlayerSupported)
            {
                StartDropThrough();
                return;
            }
        }

        if (_canJump)
        {
            PerformInitialJump();
        }
        else
        {
            _jumpBufferCounter = jumpBufferTime;
        }
    }

    // ¡NUEVO HANDLER para cuando se SUELTA el botón!
    private void HandleJumpReleased()
    {
        if (_isHoldingJump)
        {
            StopJumpHold();
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
                _isHoldingJump = false; // Si tocamos suelo, dejamos de sostener
            }
        }
        else
        {
            _timeSinceLastGroundTouch += Time.fixedDeltaTime;
            _canJump = _timeSinceLastGroundTouch < coyoteTime;
        }

        if (_canJump && _jumpBufferCounter > 0)
        {
            PerformInitialJump();
            _jumpBufferCounter = 0;
        }
    }

    private void PerformInitialJump()
    {
        AudioManager.Instance.PlaySfx(jumpSound);
        
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        _canJump = false;
        _isHoldingJump = true; // ¡Empezamos a sostener!
        _jumpHoldTimer = 0f;
        _timeSinceLastGroundTouch = coyoteTime + 1f;
    }

    // Nueva lógica para aplicar la fuerza mientras se sostiene
    private void HandleJumpHold()
    {
        if (_isHoldingJump)
        {
            _jumpHoldTimer += Time.fixedDeltaTime;
            if (_jumpHoldTimer < jumpHoldDuration)
            {
                _rb.AddForce(Vector2.up * jumpHoldForce);
            }
            else
            {
                // Se acabó el tiempo, dejamos de sostener automáticamente
                _isHoldingJump = false;
            }
        }
    }

    // Nueva lógica para cuando se suelta el botón
    private void StopJumpHold()
    {
        _isHoldingJump = false;
        // Cortamos la velocidad vertical para una respuesta más rápida
        if (_rb.linearVelocity.y > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.5f);
        }
    }

    // Resto de métodos sin cambios
    private void PerformJump()
    {
        PerformInitialJump(); // Redirigir al nuevo método
    }

    private void ApplyGravityModifiers()
    {
        _rb.gravityScale = _rb.linearVelocity.y < 0 
            ? _initialGravityScale * fallGravityMultiplier 
            : _initialGravityScale;
    }
    
    private OneWayPlatform GetCurrentOneWayPlatform()
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            transform.position, 
            platformDetectionRadius
        );
        
        foreach (var collider in nearbyColliders)
        {
            OneWayPlatform platform = collider.GetComponent<OneWayPlatform>();
            if (platform != null)
            {
                return platform;
            }
        }
        
        return null;
    }
    
    private void StartDropThrough()
    {
        _dropThroughTimer = dropThroughDuration;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -3f);
        Debug.Log("🔽 Drop through iniciado");
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, platformDetectionRadius);
    }
}