using UnityEngine;

public class EnemyDistance : EnemyClass
{
   // Distancia mínima para atacar al jugador
  [SerializeField] private Transform attackPoint; // Punto de ataque del enemigo (opcional)
  [SerializeField] private GameObject attack; // Prefab o referencia al ataque (opcional)
  [SerializeField] private float attackCooldown = 2f; // Tiempo de espera entre ataques
  [SerializeField] private GameObject Player; // Referencia al jugador

  private float lastAttackTime;

  void Start()
  {
        distanceToPlayer = 8f;
        lastAttackTime = -attackCooldown; // Permite atacar inmediatamente al iniciar si está en rango
  }

  void Update()
  {
    if (Player == null) return;

    float distance = Vector3.Distance(transform.position, Player.transform.position);

    if (distance <= distanceToPlayer && Time.time >= lastAttackTime + attackCooldown)
    {
      AttackPlayer();
      lastAttackTime = Time.time;
    }
  }

  private void AttackPlayer()
  {


    if (attack != null && attackPoint != null && Player != null)
    {
      GameObject projectile = Instantiate(attack, attackPoint.position, Quaternion.identity);
      EnemyAttack enemyAttack = projectile.GetComponent<EnemyAttack>();
      if (enemyAttack != null)
      {
        enemyAttack.SetTarget(Player.transform);
      }
    }
  }

}
