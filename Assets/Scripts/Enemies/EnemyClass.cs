using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] ElementType[] ElementDefense;
    private int health = 100; // Salud del enemigo, puedes ajustarla según sea necesario

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
        {
            // Lógica para manejar el daño normal
            damage = Mathf.RoundToInt(attackData.baseDamage); // Daño normal
        }
        health -= damage; // Reducir la salud del enemigo
    }


    private bool IsWeakAgainst(ElementType attackElement)
    {
        // Verifica si el tipo de elemento del ataque es débil contra la defensa del enemigo
        foreach (var defenseElement in ElementDefense)
        {
            if (IsWeakAgainst(attackElement, defenseElement))
            {
                return true;
            }
        }
        return false;
    }
    

    private bool IsWeakAgainst(ElementType attackElement, ElementType defenseElement)
    {
        // Implementa la lógica para determinar si un tipo de elemento es débil contra otro
        // Por ejemplo, podrías usar una tabla de debilidades
        switch (attackElement)
        {
            case ElementType.Fire:
                return defenseElement == ElementType.Water;
            case ElementType.Water:
                return defenseElement == ElementType.Electric;
            case ElementType.Electric:
                return defenseElement == ElementType.Fire;
            default:
                return false; // Por defecto, no hay debilidad
        }
    }
}
