using System;
using UnityEngine;

public class PlayerInk : MonoBehaviour
{
    public static event Action OnInkChange;
    private int _currentInk = 1000;
    private int _maxInk = 100;

    public bool puedoRecargar = false;

    private int _reloadInkAmount = 4;

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
        if (_reloadInkAmount <= 0) return;
        CurrentInk = MaxInk;
        _reloadInkAmount--;
    }


    public void ReloadUses()
    {
        if (puedoRecargar)
        {
            _reloadInkAmount = 4;
            CurrentInk = MaxInk;    
        }
       
    }
}
