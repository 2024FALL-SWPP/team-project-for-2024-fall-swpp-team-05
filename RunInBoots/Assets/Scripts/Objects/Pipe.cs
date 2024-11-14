using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Pipe : InteractableObject
{
    public int pipeID;
    public int targetPipeID;

    protected override void Start()
    {
        base.Start();

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

    protected override void OnInteract()
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
            GameManager.Instance.enteredPipeID = targetPipeID;
            GameManager.Instance.isComingFromPipe = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            LoadScene(targetStage, targetIndex);
        }
        else
        {
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Pipe targetPipe = GameUtils.FindPipeByID(GameManager.Instance.enteredPipeID);
        if (targetPipe != null)
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

    private bool ParseSceneName(string sceneName, out int stage, out int index)
    {
        stage = 0;
        index = 0;

        string[] parts = sceneName.Split('_');
        if (parts.Length == 3 && parts[0] == "Stage" && int.TryParse(parts[1], out stage) && int.TryParse(parts[2], out index))
        {
            return true;
        }

        return false;
    }
}
