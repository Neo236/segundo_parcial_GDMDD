using UnityEngine;

public class JefeFantasma :EnemyDistance
{
    [SerializeField] Transform[] ataqueLocacion;
    private int da�oCont=0; 

    protected override void Awake()
    {
    
    base.Awake();
        
    }
    public override void TakeDamage(AttackData attackData)
    {
        Debug.Log("Tomo da�o Jefe");
        base.TakeDamage(attackData);
        if (da�oCont == ataqueLocacion.Length)
        {
            da�oCont = 0;
        
        }
        transform.position = ataqueLocacion[da�oCont].localPosition;
        da�oCont++;
        Debug.Log("Teleportado");
    }



}

