using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catnip : Interactable
{
    public int catnipID;

    private void Start()
    {
        gameObject.name = "Catnip_" + catnipID;
        Initialize();
    }

    public override void Initialize()
    {
        if (GameManager.Instance.GetCurrentStageState()?.isCatnipCollected[catnipID - 1] ?? false)
        {
            gameObject.SetActive(false);
        }
    }

    protected override void OnInteract(GameObject interactor)
    {
        // GameManager.Instance.GetCurrentStageState().CollectCatnipInStageState(catnipID);
        GameManager.Instance.CollectCatnipWithEvent(catnipID);
        // gameObject.SetActive(false);
    }
}
