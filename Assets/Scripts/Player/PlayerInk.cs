using System;
using UnityEngine;


public class PlayerInk : MonoBehaviour
{
    public static event Action OnInkChange;
    private int _currentInk = 1000;
    private int _maxInk = 100;

    public bool puedoRecargar = false;

    public int _reloadInkAmount = 3;

    // Cooldown entre recargas
    public float reloadCooldown = 1f; // duración del cooldown en segundos
    private float _lastReloadTime = -Mathf.Infinity; // almacena el último tiempo de recarga


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
        // Si no pasó el cooldown, no hace nada
        if (Time.time - _lastReloadTime < reloadCooldown)
            return;

        // Si no quedan recargas disponibles, tampoco hace nada
        if (_reloadInkAmount <= 0)
            return;

        // Realiza la recarga
        CurrentInk = MaxInk;
        _reloadInkAmount--;

        // Registra el tiempo de la última recarga
        _lastReloadTime = Time.time;

        Debug.Log($"Recargado. Cantidad de recargas restantes: {_reloadInkAmount}");
    }


    public void ReloadUses()
    {
        if (puedoRecargar)
        {
            _reloadInkAmount = 3;
            CurrentInk = MaxInk;
        }
    }
    
    public void ResetInk()
    {
        CurrentInk = MaxInk;
        // Opcional: si tienes un sistema de "usos" de recarga, también lo reseteas aquí.
        // _reloadInkAmount = 4; 
        Debug.Log("Tinta del jugador reseteada al máximo.");
    }
}
