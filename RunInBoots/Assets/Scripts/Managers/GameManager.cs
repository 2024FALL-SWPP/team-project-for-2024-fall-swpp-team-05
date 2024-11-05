using UnityEngine;
using Cinemachine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject playerPrefab;
    private GameObject playerInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬 전환 시 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnPlayer(Vector3 position)
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }
        playerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
        // 카메라의 Follow 타겟 설정
        Cinemachine.CinemachineVirtualCamera vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = playerInstance.transform;
        }
    }
}
