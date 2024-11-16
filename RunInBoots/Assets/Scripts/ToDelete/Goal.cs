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
        // "Player" �±׸� ���� ������Ʈ�� �浹�ϸ� �̺�Ʈ ȣ��
        if (other.CompareTag("Player"))
        {
            onStageClear.Invoke();
            Debug.Log("Player reached the goal!");
        }
    }

    private void ClearStage()
    {
        // ���� ���������� �����ϴ� ���� ���� �ִ��� Ȯ��
        string nextSceneName = $"Stage_{currentStage + 1}_{currentIndex}";

        // �ش� ���� �ε� ������ �������� Ȯ��
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
