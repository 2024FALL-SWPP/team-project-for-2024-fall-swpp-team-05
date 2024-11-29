using TMPro;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public void Start()
    {
        //Initialize();
    }

    public void Initialize()
    {
        GameManager.Instance.GetCurrentStageState().SpawnPlayer(transform.position);
        GameManager.Instance.GetCurrentStageState().UpdateRespawnPosition(transform.position);
        gameObject.SetActive(false);
    }
}
