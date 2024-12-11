using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsAnimation : MonoBehaviour
{
    public void OnCreditsComplete()
    {
        GameManager.Instance.currentState.Exit(eExitState.GameClear);
    }
}
