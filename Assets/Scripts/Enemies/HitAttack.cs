using UnityEngine;
using System;
using Unity.VisualScripting;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class HitAttack:MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    public float Lifetime => lifetime;
    [SerializeField] private int damage = 10;
    [SerializeField] private Rigidbody2D body;
    private Vector2 posicionConEnemigo;
    


    private void Awake()
    {
        posicionConEnemigo= transform.localPosition;
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
       
            Debug.Log("Player hit by enemy attack!");

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Aplica daño al jugador
            }

           
        }
    }
    private void OnEnable()
    {
        transform.localPosition = posicionConEnemigo;
        Invoke("Desactivarse",lifetime);
        
    }
    private void Desactivarse()
    {

        gameObject.SetActive(false);
    }
    public void Activarse()
    {
        gameObject.SetActive(true);
    }
   
}
