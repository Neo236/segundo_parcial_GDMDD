using UnityEngine;

public class PlayerHealthj : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int maxHealth = 100;
    public int currentHealth;

    public int currentInk;
    public int maxInk = 100;
    void Start()
    {
        currentHealth = maxHealth;
        currentInk = maxInk;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
