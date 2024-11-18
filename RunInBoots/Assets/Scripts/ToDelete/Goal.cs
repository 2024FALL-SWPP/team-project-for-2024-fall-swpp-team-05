using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public UnityEvent onStageClear;
    public int currentStage;
    public int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (onStageClear != null)
        {
            onStageClear = new UnityEvent();
        }
        onStageClear.AddListener(ClearStage);
    }

    private void OnTriggerEnter(Collider other)
    {
        // "Player" 태그를 가진 오브젝트가 충돌하면 이벤트 호출
        if (other.CompareTag("Player"))
        {
            onStageClear.Invoke();
            Debug.Log("Player reached the goal!");
        }
    }

    private void ClearStage()
    {
        // 다음 스테이지에 대응하는 레벨 씬이 있는지 확인
        string nextSceneName = $"Stage_{currentStage + 1}_{currentIndex}";

        // 해당 씬이 로드 가능한 상태인지 확인
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            Debug.Log($"Loading next stage: {nextSceneName}");
        }
        else
        {
            Debug.Log("No next stage available. Ending current stage.");
        }
    }
}
