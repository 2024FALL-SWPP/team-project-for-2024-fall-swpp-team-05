using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 이 오브젝트가 씬 전환 시에도 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            // 로딩 진행 상황을 표시하거나 다른 처리를 할 수 있음
            yield return null;
        }
    }
}
