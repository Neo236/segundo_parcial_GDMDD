using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Attacks/Basic Attack")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public float baseDamage;
    public ElementType elementType;
    public GameObject visualEffect;

    public int inkCost;
}


