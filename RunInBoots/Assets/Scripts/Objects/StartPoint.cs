using UnityEngine;

public class StartPoint : MonoBehaviour
{
    void Start()
    {
        // 플레이어 생성
        GameManager.Instance.SpawnPlayer(transform.position);
        // 자신은 비활성화
        gameObject.SetActive(false);
    }
}
