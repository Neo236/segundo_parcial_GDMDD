using System;
using UnityEngine;

public class InkSelector : MonoBehaviour
{
    public static event Action<ElementType> OnInkTypeChanged;
    public ElementType[] availableInks;
    public ElementType currentInk;

    private int _currentInkIndex = 0;
    private void Awake()
    {
        if (availableInks.Length > 0)
        {
            SelectInk(0);
        }
        else
        {
            Debug.LogWarning("No inks available");
        }
        
        AttackInput.OnSwitchInkButtonPressed += SelectNextInk;
    }

    private void OnDestroy()
    {
        AttackInput.OnSwitchInkButtonPressed -= SelectNextInk;
    }

    public void SelectInk(int index)
    {
        if (index >= 0 && index < availableInks.Length)
        {
            _currentInkIndex = index;
            currentInk = availableInks[index];
            OnInkTypeChanged?.Invoke(currentInk);
            Debug.Log("Selected Ink: " + currentInk);
        }
        else
        {
            Debug.LogWarning("Invalid ink index: " + index);
        }
    }
    public void SelectNextInk()
    {
        int nextIndex = (_currentInkIndex + 1) % availableInks.Length;
        SelectInk(nextIndex);
    }
}