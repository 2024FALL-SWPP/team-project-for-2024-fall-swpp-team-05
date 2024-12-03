using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    // �Է��� stage�� index�� �ش��ϴ� �� �ε�. ���� �� return false
    public static bool LoadTargetStage(int targetStage, int targetIndex)
    {
        Debug.Log($"Loading target stage: {targetStage}, index: {targetIndex}");
        string nextSceneName = $"Stage_{targetStage}_{targetIndex}";

        if (SceneUtility.GetBuildIndexByScenePath(nextSceneName) != -1)
        {
            SceneManager.LoadScene(nextSceneName);
            Debug.LogWarning($"Loading next stage: {nextSceneName}");
            return true;
        }
        else
        {
            Debug.LogWarning("No next stage available. Ending current stage.");
            return false;
        }
    }

    public static void LoadCurrentScene()
    {
        Debug.LogWarning($"Loading current scene");
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public static void LoadTitleScene()
    {
        Debug.LogWarning($"Loading title scene");
        SceneManager.LoadScene("TitleScene");
    }
}
