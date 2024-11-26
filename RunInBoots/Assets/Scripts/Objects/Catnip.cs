using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catnip : MonoBehaviour
{
    public int catnipID;

    private void Start()
    {
        gameObject.name = "Catnip_" + catnipID;
        if (GameManager.Instance.isCatnipCollected[catnipID - 1])
        {
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectCatnipWithEvent(catnipID);
        }
    }
}
