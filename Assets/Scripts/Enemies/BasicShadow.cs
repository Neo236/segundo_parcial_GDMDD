using System;
using Unity.VisualScripting;
using UnityEngine;

public class BasicShadow : EnemyClass
{
    [SerializeField] private float distanceToPlayer ;
    [SerializeField] private GameObject player;
    [SerializeField] private float velocidad;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown = 2f; // Tiempo de espera entre ataques
    [SerializeField] private GameObject attack; // Prefab o referencia al ataque (opcional)
    [SerializeField] private Transform attackPoint; // Punto de ataque del enemigo (opcional)
    private Rigidbody2D rb;
    private float lastAttackTime;
    private Vector3 objetivo;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    private void Update()
    {

        if (player != null && DistanciaObjetivo() <= distanceToPlayer && !(DistanciaObjetivo() <attackDistance))
        {
            Moverse();
        }
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }
    private void Moverse()
    {
        Console.WriteLine("Moviendose");
        Vector2 direccion = new Vector2(player.transform.position.x - transform.position.x, 0);

        rb.MovePosition(rb.position + Time.fixedDeltaTime * velocidad * direccion);

    }
    private float DistanciaObjetivo()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance;
    }
    private void AttackPlayer()
    {


        if (attack != null && attackPoint != null && player != null)
        {
            Console.WriteLine("Atacando");
            GameObject projectile = Instantiate(attack, attackPoint.position, Quaternion.identity);
            EnemyAttack enemyAttack = projectile.GetComponent<EnemyAttack>();
           
        }
    }

}
