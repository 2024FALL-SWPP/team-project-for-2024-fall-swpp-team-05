using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscene : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnButtonClicked()
    {
        GameManager.Instance.StartNewStage(100);
    }
}
