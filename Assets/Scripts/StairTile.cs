using System;
using UnityEngine;
using UnityEngine.Serialization;

public class StairTile : MonoBehaviour
{
    [Header("Stair Configuration")]
    [SerializeField] private float playerClimbingSpeed = 5f;
    
    private GameObject _player;
    private Rigidbody2D _playerRigidbody;
    private bool _isPlayerOnTile;
    private float _verticalInput;

    private void Start()
    {
        // Validación de GameManager
        if (GameManager.Instance?.playerObject == null)
        {
            Debug.LogError("StairTile: No se pudo obtener el player object desde GameManager");
            enabled = false;
            return;
        }
        
        _player = GameManager.Instance.playerObject;
        _playerRigidbody = _player.GetComponent<Rigidbody2D>();
        
        // ✅ CORREGIDO: Quitar el punto y coma extra
        MovementInput.OnMoveInput += HandleMoveInput;
    }

    private void OnDisable()
    {
        MovementInput.OnMoveInput -= HandleMoveInput;
    }
    
    private void HandleMoveInput(Vector2 moveInput)
    {
        // Guardamos solo el componente vertical, que es el que nos interesa.
        _verticalInput = moveInput.y;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _player)
        {
            _isPlayerOnTile = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == _player)
        {
            _isPlayerOnTile = false;
        }
    }

    private void HandleVerticalMovement(float movement)
    {
        _verticalInput = movement;
    }
    
    // ✅ CORREGIDO: Quitar parámetro innecesario
    private void ClimbStair()
    {
        if (_verticalInput > 0 && _isPlayerOnTile && _player != null)
        {
            // Método preferido: Usar Rigidbody2D si está disponible
            if (_playerRigidbody != null)
            {
                // Mantener velocidad horizontal, aplicar velocidad vertical
                Vector2 newVelocity = new Vector2(_playerRigidbody.linearVelocity.x, playerClimbingSpeed);
                _playerRigidbody.linearVelocity = newVelocity;
            }
            else
            {
                // Fallback: Transform.Translate
                _player.transform.Translate(Vector3.up * playerClimbingSpeed * Time.deltaTime);
            }
        }
    }
    
    private void Update()
    {
        ClimbStair(); // ✅ Ya no necesita parámetro
    }
}