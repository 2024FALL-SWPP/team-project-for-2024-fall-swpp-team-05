using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, ILevelObject
{
    public abstract void Initialize();

    protected abstract void OnInteract(GameObject interactor);

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            OnInteract(other.gameObject);
        }
    }
}
