using UnityEngine;

public class StartPoint_old : MonoBehaviour
{
    void Start()
    {
        // 플레이어 생성
        GameManager_old.Instance.SpawnPlayer(transform.position);
        // 자신은 비활성화
        gameObject.SetActive(false);
    }
}
