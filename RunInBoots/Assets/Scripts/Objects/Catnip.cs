using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catnip : MonoBehaviour
{
    public int catnipID;

    private void Start()
    {
        if (GameManager.Instance.GetCurrentStageState().isCatnipCollected[catnipID - 1])
        {
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GetCurrentStageState().CollectCatnipInStageState(catnipID);
            gameObject.SetActive(false);
        }
        
    }
}
