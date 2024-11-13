using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Pipe : MonoBehaviour
{
    public int pipeID;
    public int targetPipeID;
    public int targetStage;
    public int targetIndex;
    public UnityEvent onInteract;


    private LevelLoader levelLoader;

    void Start()
    {
        // 게임 시작 시 상호작용 이벤트에 토관 이동 함수 연결
        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null || levelLoader.terrainData == null)
        {
            Debug.LogError("LevelLoader 또는 terrainData를 찾을 수 없습니다.");
            return;
        }
        onInteract.AddListener(HandlePipeInteraction);

        if (GameManager.Instance.isComingFromPipe && GameManager.Instance.enteredPipeID == pipeID)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector3 targetPosition = transform.position + Vector3.right * 1.5f;
                player.transform.position = targetPosition;
                GameManager.Instance.ResetPipeData(); // 상태 초기화
            }
        }
    }

    void HandlePipeInteraction()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int currentStage, currentIndex;

        if (!ParseSceneName(sceneName, out currentStage, out currentIndex))
        {
            Debug.LogError($"현재 씬 이름에서 Stage와 Index를 파싱할 수 없습니다: {sceneName}");
            return;
        }

        if (currentStage != targetStage || currentIndex != targetIndex)
        {
            //새로운 씬으로 이동
            GameManager.Instance.enteredPipeID = targetPipeID;
            GameManager.Instance.isComingFromPipe = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene($"Stage_{targetStage}_{targetIndex}");
        }
        else
        {
            // 동일한 씬 내에서 이동
            Pipe targetPipe = GameUtils.FindPipeByID(targetPipeID);

            if (targetPipe != null)
            {
                Vector3 targetPosition = targetPipe.transform.position + Vector3.right * 1.5f;
                GameManager.Instance.SpawnPlayer(targetPosition);
            }
            else
            {
                Debug.LogError($"ID가 {targetPipeID}인 파이프를 찾을 수 없습니다.");
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Pipe targetPipe = GameUtils.FindPipeByID(GameManager.Instance.enteredPipeID);
        if(targetPipe != null)
        {
            Vector3 targetPosition = targetPipe.transform.position + Vector3.right * 1.5f;
            GameManager.Instance.SpawnPlayer(targetPosition);
        }
        else
        {
            Debug.LogError("다음 씬에서 target Pipe를 찾을 수 없습니다.");
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onInteract.Invoke();
            Debug.Log("Player meet Pipe");
        }
    }

    private bool ParseSceneName(string sceneName, out int stage, out int index)
    {
        stage = 0;
        index = 0;

        // 씬 이름이 "Stage_{stage}_{index}" 형태인지 확인
        string[] parts = sceneName.Split('_');
        if (parts.Length == 3 && parts[0] == "Stage" && int.TryParse(parts[1], out stage) && int.TryParse(parts[2], out index))
        {
            return true;
        }

        return false;
    }
}
