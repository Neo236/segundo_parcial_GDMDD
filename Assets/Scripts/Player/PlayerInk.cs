using System;
using UnityEngine;

public class PlayerInk : MonoBehaviour
{
    public static event Action OnInkChange;
    private int _currentInk = 1000;
    private int _maxInk = 100;
    
    public int CurrentInk
    {
        get => _currentInk;
        set
        {
            int newValue = Mathf.Clamp(value, 0, MaxInk);
            if (_currentInk != newValue)
            {
                _currentInk = newValue;
                OnInkChange?.Invoke();
            }
        }
    }

    public int MaxInk => _maxInk;
    
    public void RefillInk()
    {
        CurrentInk = MaxInk;
    }
}
