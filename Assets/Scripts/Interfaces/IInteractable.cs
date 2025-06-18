using Unity.VisualScripting;
using UnityEngine;

namespace Interfaces
{
    public interface IInteractable
    {
        void OnTriggerEnter(Collider other);
        void OnTriggerExit(Collider other);
    }
}