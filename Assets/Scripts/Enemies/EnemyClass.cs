using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] ElementType[] ElementDefense;

    [SerializeField] ElementType _weakness; // Datos del ataque que el enemigo puede recibir
    [SerializeField] private int health = 100; // Salud del enemigo, puedes ajustarla según sea necesario


    public bool IsDead { get; private set; } = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(AttackData attackData)
    {
        // Implementar lógica de daño al enemigo
        // Por ejemplo, reducir la salud del enemigo según el ataque recibido
        // y aplicar efectos visuales o de sonido si es necesario.
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
        if (health <= 0)
        {
            Die(); // Lógica para manejar la muerte del enemigo
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
    
    private void Die()
    {
        // Lógica para manejar la muerte del enemigo
        // Por ejemplo, podrías reproducir una animación de muerte, soltar objetos, etc.
        Debug.Log("Enemy has died.");
        IsDead = true; // Marcar al enemigo como muerto
        Destroy(gameObject); // Destruye el objeto del enemigo
    }
}
