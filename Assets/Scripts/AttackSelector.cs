using UnityEngine;

public class AttackSelector : MonoBehaviour
{
    public AttackData[] availableAttacks;
    public AttackData selectedAttack;

    private int currentAttackIndex = 0;

    void Start()
    {
        if (availableAttacks.Length > 0)
        {
            SelectAttack(0);
        }
    }

    void Update()
    {
        // TEMPORAL: Cambiar con la tecla "Q"
        
    }

    public void SelectAttack(int index)
    {
        if (index >= 0 && index < availableAttacks.Length)
        {
            currentAttackIndex = index;
            selectedAttack = availableAttacks[index];
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
