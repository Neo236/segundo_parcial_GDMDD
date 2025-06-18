using System;
using UnityEngine;

public class EnemyFollow:EnemyClass

{
    [SerializeField] private float distanceToPlayer = 100f; 
    [SerializeField] private GameObject player; 
    [SerializeField] private float velocidad;
    private Rigidbody2D rb;
   
    private Vector3 objetivo;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        
        if (player!=null && DistanciaObjetivo() <=distanceToPlayer)
        {
            Moverse();
        }
       
    }
    private void Moverse()
    {
        Console.WriteLine("Moviendose");
        Vector2 direccion = new Vector2(player.transform.position.x-transform.position.x,0);

        rb.MovePosition(rb.position + Time.fixedDeltaTime * velocidad * direccion);

    }
    private float DistanciaObjetivo()
    {
         float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance;
    }


}
