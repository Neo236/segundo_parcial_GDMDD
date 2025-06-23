using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class BasicShadow : EnemyClass
{


    
    [SerializeField] private float velocidad;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown = 2f; // Tiempo de espera entre ataques
    [SerializeField] private GameObject attack;
    [SerializeField] private HitAttack attackScript; // Prefab o referencia al ataque (opcional)
    [SerializeField] private GameObject attackPoint; // Punto de ataque del enemigo (opcional)
    [SerializeField]private Vector3 attackPosition;
    [SerializeField] private Vector3 scaleAttack;
    
    private Rigidbody2D rb;
    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
        attack = Instantiate(attack,transform);
        rb = GetComponent<Rigidbody2D>();
        attackScript = GetComponentInChildren<HitAttack>();

    }
    private void Update()
    {
        if (!(animator.GetBool("Death")))
        {
            if (DetectarJugador())
            {
                if (Time.time >= lastAttackTime + attackCooldown && !(DistanciaObjetivo() <= attackDistance))
                {
                    VoltearAlJugador();
                    Moverse();


                }
                if (DistanciaObjetivo() <= attackDistance && Time.time >= lastAttackTime + attackCooldown)
                {
                    animator.SetBool("Moviendose", false);
                    VoltearAlJugador();
                    EmpezarAtaque();
                    lastAttackTime = Time.time;
                }
            }
        }
    }
    private void Moverse()
    {
        jugDireccion.y = 0;
        if (_groundCheck.indivGrounded[2] && volteadoReal || _groundCheck.indivGrounded[0] && !volteadoReal)
        {
            animator.SetBool("Moviendose", true);
            rb.MovePosition(rb.position + Time.fixedDeltaTime * velocidad * jugDireccion);
           
            Debug.Log("MOVIENDOSE");

        }
        else
        {
            animator.SetBool("Moviendose",false);
        }
        
       
    }
    private float DistanciaObjetivo()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance;
    }
     override  public void AttackPlayer()
    {
        

        if (attackScript != null && attackScript != null && player != null)
        {
            Console.WriteLine("Atacando");
          
           attackScript.Activarse();
           
        }
    }

}
