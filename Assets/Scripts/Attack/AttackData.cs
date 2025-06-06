using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Attacks/Basic Attack")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public float baseDamage;
    public ElementType elementType;
    public GameObject visualEffect;
    public Sprite attackIcon; // Add this field for UI representation
    public int inkCost;
    public AudioClip soundEffect;
}