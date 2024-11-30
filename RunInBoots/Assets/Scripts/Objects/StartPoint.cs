using TMPro;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        //Vector3 spawnPosition = transform.position + Vector3.up * 1.0f;
        //GameManager.Instance.StartNewStage();
        //GameManager.Instance.SpawnPlayer(spawnPosition, !GameManager.Instance.isRespawnPositionSetted);

        if (!GameManager.Instance.isRespawnPositionSetted)
        {
            // 초기 스폰 위치를 설정
            Vector3 spawnPosition = transform.position + Vector3.up * 1.0f;
            GameManager.Instance.StartNewStage();
            //GameManager.Instance.SpawnPlayer(spawnPosition);
            GameManager.Instance.SpawnPlayerWithEvent(spawnPosition);
            GameManager.Instance.UpdateRespawnPosition(spawnPosition);
        }
        else
        {
            // 리스폰 위치가 이미 설정된 경우 캐릭터만 스폰
            // GameManager.Instance.SpawnPlayer(GameManager.Instance.respawnPosition);
            GameManager.Instance.SpawnPlayerWithEvent(GameManager.Instance.respawnPosition);
        }

    }
}
