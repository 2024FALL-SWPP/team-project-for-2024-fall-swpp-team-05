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
            // �ʱ� ���� ��ġ�� ����
            Vector3 spawnPosition = transform.position + Vector3.up * 1.0f;
            GameManager.Instance.StartNewStage();
            //GameManager.Instance.SpawnPlayer(spawnPosition);
            GameManager.Instance.SpawnPlayerWithEvent(spawnPosition);
            GameManager.Instance.UpdateRespawnPosition(spawnPosition);
        }
        else
        {
            // ������ ��ġ�� �̹� ������ ��� ĳ���͸� ����
            // GameManager.Instance.SpawnPlayer(GameManager.Instance.respawnPosition);
            GameManager.Instance.SpawnPlayerWithEvent(GameManager.Instance.respawnPosition);
        }

    }
}
