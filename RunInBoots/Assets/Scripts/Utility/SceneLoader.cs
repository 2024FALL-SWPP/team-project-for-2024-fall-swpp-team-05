using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    // �Է��� stage�� index�� �ش��ϴ� �� �ε�. ���� �� return false
    public static bool LoadTargetStage(int targetStage, int targetIndex)
    {
        string nextSceneName = $"Stage_{targetStage}_{targetIndex}";

        if (SceneUtility.GetBuildIndexByScenePath(nextSceneName) != -1)
        {
            SceneManager.LoadScene(nextSceneName);
            Debug.Log($"Loading next stage: {nextSceneName}");
            return true;
        }
        else
        {
            Debug.Log("No next stage available. Ending current stage.");
            return false;
        }
    }

    public static void LoadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public static void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
