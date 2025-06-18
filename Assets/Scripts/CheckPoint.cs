using Interfaces;
using UnityEngine;

public class CheckPoint : MonoBehaviour, IInteractable
{
    private bool canBeActivated;
    public bool CanBeActivated { get; set; }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canBeActivated = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canBeActivated = false;
        }
    }
}
