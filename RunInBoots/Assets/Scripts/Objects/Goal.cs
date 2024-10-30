using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public UnityEvent onInteract;

    void Start()
    {
        onInteract.AddListener(HandleGoalInteraction);
    }

    void HandleGoalInteraction()
    {
        // 다음 스테이지가 있으면 해당 씬 로드
        string nextTerrainIndex = TerrainDataLoader.Instance.terrainData.terrainIndex + 1;
        string nextSceneName = "Stage_" + TerrainDataLoader.Instance.terrainData.stage + "_" + nextTerrainIndex;

        // 씬이 존재하는지 확인
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneLoadManager.Instance.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("No more stages.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onInteract.Invoke();
        }
    }
}
