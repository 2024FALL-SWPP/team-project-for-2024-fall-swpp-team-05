using UnityEngine;
using UnityEngine.Events;

public class Pipe : MonoBehaviour
{
    public int pipeID;
    public string targetTerrainIndex;
    public int targetPipeID;
    public UnityEvent onInteract;

    void Start()
    {
        // 게임 시작 시 상호작용 이벤트에 토관 이동 함수 연결
        onInteract.AddListener(HandlePipeInteraction);
    }

    void HandlePipeInteraction()
    {
        // 토관 이동 로직 구현
        if (targetTerrainIndex != TerrainDataLoader.Instance.terrainData.terrainIndex)
        {
            // 다른 지형으로 이동
            SceneLoadManager.Instance.LoadScene("Stage_" + TerrainDataLoader.Instance.terrainData.stage + "_" + targetTerrainIndex);
        }
        else
        {
            // 같은 지형 내에서 이동
            // 연결된 토관의 위치로 플레이어 이동
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
