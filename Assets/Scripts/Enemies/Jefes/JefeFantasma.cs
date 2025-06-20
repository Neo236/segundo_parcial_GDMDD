using UnityEngine;

public class JefeFantasma :EnemyDistance
{
    private int cantAtaques=3;




    new protected void AttackPlayer()
    {


        if (attack != null && attackPoint != null && player != null)
        {
            GameObject[] Proyectiles=new GameObject[cantAtaques];
        
            for (global::System.Int32 i = 0; i < cantAtaques; i++)
            {
                EnemyAttack enemyAttack = Proyectiles[i].GetComponent<EnemyAttack>();
                if (enemyAttack != null)
                {
                    Proyectiles[i] = Instantiate(attack, attackPoint.transform.position, Quaternion.identity);
                 
                    enemyAttack.SetTarget(player.transform);
                }
            }
           
        }
    }
}
