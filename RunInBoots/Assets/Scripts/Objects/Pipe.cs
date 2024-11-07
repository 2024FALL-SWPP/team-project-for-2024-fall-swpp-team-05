using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Pipe : MonoBehaviour
{
    public int pipeID;
    public string targetTerrainIndex;
    public int targetPipeID;
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
    }

    void HandlePipeInteraction()
    {
        PipeData currentPipeData = levelLoader.terrainData.pipeConnections.pipeList.Find(pipe => pipe.pipeID == pipeID);
        if(currentPipeData == null )
        {
            Debug.LogError($"pipeID {pipeID}에 해당하는 PipeData를 찾을 수 없습니다.");
            return;
        }

        if (currentPipeData.targetTerrainIndex != levelLoader.index)
        {
            // 다른 씬으로 이동
            GameManager.Instance.enteredPipeID = currentPipeData.targetPipeID;
            GameManager.Instance.isComingFromPipe = true;

            string stage = levelLoader.stage;
            SceneManager.LoadScene($"Stage_{stage}_{currentPipeData.targetTerrainIndex}");
        }
        else
        {
            // 동일한 씬 내에서 이동
            Pipe targetPipe = GameUtils.FindPipeByID(currentPipeData.targetPipeID);

            if(targetPipe != null)
            {
                Vector3 targetPosition = targetPipe.transform.position + Vector3.right * 1.5f;
                GameObject player = GameObject.FindWithTag("Player");
                if(player != null)
                {
                    player.transform.position = targetPosition;
                }
                else
                {
                    Debug.LogError("Player를 찾을 수 없습니다.");
                }
            }
            else 
            {
                Debug.LogError($"ID가 {targetPipeID}인 파이프를 찾을 수 없습니다.");
            }
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
