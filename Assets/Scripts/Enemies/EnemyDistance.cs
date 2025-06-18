using Unity.VisualScripting;
using UnityEngine;

public class EnemyDistance : EnemyClass
{
   // Distancia mínima para atacar al jugador
  [SerializeField] private GameObject attackPoint; // Punto de ataque del enemigo (opcional)
    [SerializeField] private Vector3 attackPointPos;
  [SerializeField] private GameObject attack; // Prefab o referencia al ataque (opcional)
  [SerializeField] private float attackCooldown = 2f; // Tiempo de espera entre ataques
 

  private float lastAttackTime;
    protected override void Awake()
    {
        base.Awake();
        attackPoint = new GameObject("AttackSpawner");
        attackPoint.transform.SetParent(transform);
        attackPoint.transform.localPosition = attackPointPos;
    }
    protected override void Start()
  {
        base.Start();
        distanceToPlayer = 8f;
        lastAttackTime = -attackCooldown; // Permite atacar inmediatamente al iniciar si está en rango
  }

  void Update()
  {
    if (player == null) return;

 

    if (DetectarJugador() && Time.time >= lastAttackTime + attackCooldown)
    {
            VoltearAlJugador();
      AttackPlayer();
      lastAttackTime = Time.time;
    }
  }

  private void AttackPlayer()
  {


    if (attack != null && attackPoint != null && player != null)
    {
      GameObject projectile = Instantiate(attack, attackPoint.transform.position, Quaternion.identity);
      EnemyAttack enemyAttack = projectile.GetComponent<EnemyAttack>();
      if (enemyAttack != null)
      {
        enemyAttack.SetTarget(player.transform);
      }
    }
  }

}
