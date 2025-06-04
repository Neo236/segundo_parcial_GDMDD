using UnityEngine;

public class Attack
{
    public string attackName;
    public float baseDamage;
    public ElementType elementType;
    public GameObject visualEffect;

    public virtual void ExecuteAttack()
    {
        // Daño base, puede ser modificado por el elemento luego
        Debug.Log($"{attackName} hace {baseDamage} de daño con elemento {elementType}");
    }
}
