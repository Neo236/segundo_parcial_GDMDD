using System;
using UnityEngine;

public class PlayerInk : MonoBehaviour
{
    public static event Action OnInkChange;
    
    private float _currentInk = 100f;
    private float _maxInk = 100f;
    
    public float CurrentInk
    {
        get => _currentInk;
        set
        {
            _currentInk = Mathf.Clamp(value, 0f, _maxInk);
            OnInkChange?.Invoke();
        }
    }
    
    public float MaxInk => _maxInk;
}