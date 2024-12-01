using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscene : MonoBehaviour
{
    public int stageToStart;

    private void Start()
    {
        GameManager.Instance.StartNewStage(stageToStart);
    }
}
