using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 1.0f;
        GameManager.Instance.StartNewStage();
        GameManager.Instance.SpawnPlayer(spawnPosition);
    }
}
