using UnityEngine;

public class JefeFantasma :EnemyDistance
{
    [SerializeField] Transform[] ataqueLocacion;
    private int dañoCont=0; 

    protected override void Awake()
    {
    
    base.Awake();
        
    }
    public override void TakeDamage(AttackData attackData)
    {
        base.TakeDamage(attackData);
        if (dañoCont == ataqueLocacion.Length)
        {
            dañoCont = 0;
        
        }
        transform.position = ataqueLocacion[dañoCont].position;
    }



}

