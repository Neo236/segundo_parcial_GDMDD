using UnityEngine;
using UnityEngine.UIElements;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private bool showDebugRays = true;
    
  private  float GROUND_CHECK_DISTANCE = 1.02f;
   private  float GROUND_CHECK_LEFT_OFFSET = -0.5f;
    private  float GROUND_CHECK_RIGHT_OFFSET = 0.5f;
    private const int RAY_COUNT = 3;

    private readonly Ray2D[] _groundCheckRays = new Ray2D[RAY_COUNT];
    private readonly Vector2[] _rayOffsets = new Vector2[RAY_COUNT];
    private LayerMask _groundMask;

    public bool IsGrounded { get; private set; }
    public bool[] indivGrounded;

    private void Awake()
    {
        indivGrounded = new bool[RAY_COUNT];
        InitializeRayOffsets();
        _groundMask = LayerMask.GetMask("Ground");
        if (_groundMask == 0)
        {
            Debug.LogWarning("Ground layer not found. Please ensure 'Ground' layer exists.");
        }
    }
   
    private void InitializeRayOffsets()
    {
        Debug.Log("inicializando rayos");
        _rayOffsets[0] = new Vector2(GROUND_CHECK_LEFT_OFFSET, 0f);
        _rayOffsets[1] = Vector2.zero;
        _rayOffsets[2] = new Vector2(GROUND_CHECK_RIGHT_OFFSET, 0f);

        for (int i = 0; i < RAY_COUNT; i++)
        {
            _groundCheckRays[i] = new Ray2D(Vector2.zero, Vector2.down);
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        IsGrounded = false;
        Vector2 currentPosition = transform.position;
        
        for (int i = 0; i < _groundCheckRays.Length; i++)
        {
            _groundCheckRays[i].origin = currentPosition + _rayOffsets[i];
            
            bool hitGround = Physics2D.Raycast(
                _groundCheckRays[i].origin, 
                Vector2.down, 
                GROUND_CHECK_DISTANCE, 
                _groundMask
            );
            if (hitGround)
            {
                
                IsGrounded = true;
                indivGrounded[i] = true;
                if (showDebugRays)
                {
                    Debug.DrawRay(
                        _groundCheckRays[i].origin, 
                        Vector2.down * GROUND_CHECK_DISTANCE, 
                        Color.red
                    );
                }
            }
            else 
            {
                indivGrounded[i] = false;
                if (showDebugRays)
                {
                   
                    Debug.DrawRay(
                    _groundCheckRays[i].origin,
                    Vector2.down * GROUND_CHECK_DISTANCE,
                    Color.green
                );
                }
              
            }

        }
        
    }
    public void AdaptRaycastToHitbox(Collider2D collider)
    {
        if (collider == null)
        {
            Debug.LogError("Collider2D nulo al intentar adaptar raycasts.");
            return;
        }

        Bounds bounds = collider.bounds;

        // Calculamos el offset horizontal desde el centro hasta los lados
        GROUND_CHECK_LEFT_OFFSET = bounds.min.x - collider.transform.position.x;
        GROUND_CHECK_RIGHT_OFFSET = bounds.max.x - collider.transform.position.x;

        // Distancia desde el centro hasta justo debajo del collider (ajustado un poco más por seguridad)
        GROUND_CHECK_DISTANCE = (collider.transform.position.y - bounds.min.y) + 0.1f;

        InitializeRayOffsets(); // Aplicamos los nuevos valores
    }

}