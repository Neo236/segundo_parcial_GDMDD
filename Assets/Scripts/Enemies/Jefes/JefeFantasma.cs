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
        Debug.Log("Tomo daño Jefe");
        base.TakeDamage(attackData);
        if (dañoCont == ataqueLocacion.Length)
        {
            dañoCont = 0;
        
        }
        transform.position = ataqueLocacion[dañoCont].localPosition;
        dañoCont++;
        Debug.Log("Teleportado");
    }



}

