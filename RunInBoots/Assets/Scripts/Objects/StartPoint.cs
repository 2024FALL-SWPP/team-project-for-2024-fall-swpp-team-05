using TMPro;
using UnityEngine;

public class StartPoint : MonoBehaviour, ILevelObject
{
    public void Initialize()
    {
        // GameManager.Instance.GetCurrentStageState().SpawnPlayer(transform.position);
        GameManager.Instance.SpawnPlayerWithEvent(transform.position);
        GameManager.Instance.GetCurrentStageState().UpdateRespawnPosition(transform.position, true);
        gameObject.SetActive(false);
    }
}
