using System;
using UnityEngine;

public class PlayerInk : MonoBehaviour
{
    public static event Action OnInkChange;
    private float _currentInk = 100f;
    private float _maxInk = 100f;
    public float CurrentInk { get; private set; }
    public float MaxInk { get; private set; }
}
