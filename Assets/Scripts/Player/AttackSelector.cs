using UnityEngine;
using System;

public class AttackSelector : MonoBehaviour
{
    public static event Action<AttackData> OnAttackChanged;
    public AttackData[] availableAttacks;
    public AttackData selectedAttack;

    private int currentAttackIndex = 0;

    private void Awake()
    {
        AttackInput.OnSwitchAttackButtonPressed += SelectNextAttack;
    }

    private void OnDestroy()
    {
        AttackInput.OnSwitchAttackButtonPressed -= SelectNextAttack;
    }

    void Start()
    {
        if (availableAttacks.Length > 0)
        {
            SelectAttack(0);
        }
    }

    public void SelectAttack(int index)
    {
        if (index >= 0 && index < availableAttacks.Length)
        {
            currentAttackIndex = index;
            selectedAttack = availableAttacks[index];
            OnAttackChanged?.Invoke(selectedAttack);
            Debug.Log("Selected Attack: " + selectedAttack.attackName);
        }
        else
        {
            Debug.LogWarning("Invalid attack index: " + index);
        }
    }

    public void SelectNextAttack()
    {
        int nextIndex = (currentAttackIndex + 1) % availableAttacks.Length;
        SelectAttack(nextIndex);
    }
}