using UnityEngine;

public class JefeFantasma :EnemyDistance
{
    [SerializeField] Transform[] ataqueLocacion;
    private int danoCont=0; 

    protected override void Awake()
    {
    
    base.Awake();
        
    }
    public override void TakeDamage(AttackData attackData)
    {
        Debug.Log("Tomo daño Jefe");
        base.TakeDamage(attackData);
        if (danoCont == ataqueLocacion.Length)
        {
            danoCont = 0;
        
        }
        transform.position = ataqueLocacion[danoCont].localPosition;
        danoCont++;
        Debug.Log("Teleportado");
    }
    public override void Die()
    {
        base.Die();
        GameManager.Instance.TriggerEndScene();
    }



}

