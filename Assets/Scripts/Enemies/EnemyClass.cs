using System;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyClass : MonoBehaviour
{
    protected GroundCheck _groundCheck;
    [SerializeField] ElementType[] ElementDefense;
    protected Vector2 jugDireccion;
    [SerializeField] ElementType _weakness; // Datos del ataque que el enemigo puede recibir
    [SerializeField] private int health = 100; // Salud del enemigo, puedes ajustarla según sea necesario
    [SerializeField] protected GameObject player;
    [SerializeField] protected bool volteadoReal;
    [SerializeField] protected float distanceToPlayer;
    protected RaycastHit2D vision;
    [SerializeField] protected LayerMask detectionMask;
    [SerializeField] protected SpriteRenderer sprite;
    protected BoxCollider2D hitbox;
    protected Animator animator;

    protected virtual void Awake()
    {

        sprite = GetComponent<SpriteRenderer>();
        detectionMask = LayerMask.GetMask("Ground", "Player");
        player = GameObject.FindWithTag("Player");
        hitbox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
       

    }
   
    public bool IsDead { get; private set; } = false;
    protected virtual void Start()
    {
        _groundCheck = GetComponent<GroundCheck>();
        hitbox = GetComponent<BoxCollider2D>();
        _groundCheck.AdaptRaycastToHitbox(hitbox);
        ChangeColorBasedOnElementType();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void TakeDamage(AttackData attackData)
    {
      
        int damage;

        // Aquí podrías verificar el tipo de elemento del ataque y compararlo con la defensa del enemigo.
        if (IsWeakAgainst(attackData.elementType))
        {
            // Lógica para manejar el daño aumentado
            damage = Mathf.RoundToInt(attackData.baseDamage * 1.5f); // Aumenta el daño en un 50%
        }
        else
            if (IsStrongAgaint(attackData.elementType))
        {
            // Lógica para manejar el daño reducido
            damage = Mathf.RoundToInt(attackData.baseDamage * 0.5f); // Reduce el daño a la mitad
        }
        else
        {
            // Lógica para manejar el daño normal
            damage = Mathf.RoundToInt(attackData.baseDamage); // Daño normal
        }
        Debug.Log($"Enemy takes {damage} damage from {attackData.elementType} attack.");
        health -= damage; // Reducir la salud del enemigo
        animator.SetBool("Hurt",true);
        if (health <= 0)
        {
            animator.SetBool("Death", true);
       

        }

    }
    


    private bool IsWeakAgainst(ElementType attackElement)
    {
        // Verifica si el tipo de elemento del ataque es débil contra la defensa del enemigo
        return attackElement == _weakness;
    }


    private bool IsStrongAgaint(ElementType attackElement)
    {
        // Implementa la lógica para determinar si un tipo de elemento es débil contra otro
        // Por ejemplo, podrías usar una tabla de debilidades
        for (int i = 0; i < ElementDefense.Length; i++)
        {
            if (ElementDefense[i] == attackElement)
            {
                return true; // El ataque es fuerte contra el enemigo
            }
        }
        return false;
    }
    protected virtual bool DetectarJugador()

    {
        if (player == null)
        {
            player = GameManager.Instance.playerObject;
        }
        jugDireccion = (player.transform.position - transform.position).normalized;

        vision = Physics2D.Raycast(transform.position, jugDireccion, distanceToPlayer, detectionMask);


        if (vision.collider != false && vision.collider.CompareTag("Player"))
        {
            if (sprite.flipX == true)
            { sprite.flipX = false; }

            {
                if ((vision.collider.transform.position.x - transform.position.x) <= 0)
                {
                    volteadoReal = false;

                }
                else
                {
                    volteadoReal = true;
                }

            }
            return true;
        }
        else { return false; }
    }
    protected void VoltearAlJugador()
    {
        if ((volteadoReal && transform.localScale.x > 0) || (!volteadoReal && transform.localScale.x < 0))
        {
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }

    }

    public void Die()
    {
        // Lógica para manejar la muerte del enemigo
        // Por ejemplo, podrías reproducir una animación de muerte, soltar objetos, etc.
        Debug.Log("Enemy has died.");
        IsDead = true; // Marcar al enemigo como muerto
        Destroy(gameObject); // Destruye el objeto del enemigo
    }
    protected virtual void  EmpezarAtaque()
    {
        animator.SetBool("Attack", true);
    
    
    }
    virtual public void AttackPlayer()
    {
        Debug.Log("atacando");

       
    }
    private void ChangeColorBasedOnElementType()
    {
        if (sprite != null)
        {
            switch (_weakness)
            {
                case ElementType.Fire:
                    sprite.color = Color.red;
                    break;
                case ElementType.Water:
                    sprite.color =Color.blue;
                    break;
                case ElementType.Electric:
                    sprite.color = Color.yellow;
                    break;
                default:
                    sprite.color = Color.white;
                    break;
            }
        }
    }
}
