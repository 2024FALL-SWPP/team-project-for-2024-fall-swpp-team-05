using System.Collections.Generic;
using UnityEngine;

public class TestUserData : MonoBehaviour
{
    private int testStage = 1; // Default stage for testing
    private int testStage2 = 2; // Default time for testing (in seconds)
    private int testTime = 120; // Default time for testing (in seconds)
    private List<bool> testCatnipStatus = new List<bool> { true, false, true }; // Default catnip status for testing

    public UserData userdata = new UserData();
    
    private void Start()
    {
        Debug.Log("Starting TestUserData");
        Debug.Log("Loading game data...");
        userdata.LoadGameData();
        Debug.Log("Game data loaded.");
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("Test User Data");

        GUILayout.Space(10);
        GUILayout.Label("Global Data");
        if (GUILayout.Button("Set Lives to 5"))
        {
            userdata.UpdateLives(5);
            Debug.Log($"Lives set to 5. Current Lives: {userdata.lives}");
        }

        if (GUILayout.Button("Set Recent Stage to 2"))
        {
            userdata.UpdateRecentStage(2);
            Debug.Log($"Recent stage set to 2. Current Recent Stage: {userdata.recentStage}");
        }

        GUILayout.Space(10);
        GUILayout.Label("Stage Data");
        if (GUILayout.Button($"Save Stage {testStage} Data (Time: {testTime}, Catnip: {string.Join(", ", testCatnipStatus)})"))
        {
            userdata.SaveStageData(testStage, testTime, testCatnipStatus);
            Debug.Log($"Stage {testStage} data saved.");
        }

        if (GUILayout.Button($"Save Stage {testStage2} Data (Time: {testTime}, Catnip: {string.Join(", ", testCatnipStatus)})"))
        {
            userdata.SaveStageData(testStage2, testTime, testCatnipStatus);
            Debug.Log($"Stage {testStage2} data saved.");
        }

        if (GUILayout.Button($"Get Stage {testStage} Data"))
        {
            var stageData = userdata.GetStageData(testStage);
            if (stageData != null)
            {
                Debug.Log($"Stage {testStage} Data - Best Time: {stageData.bestTime}, Catnip: {string.Join(", ", stageData.catnipCollected)}");
            }
            else
            {
                Debug.Log($"No data found for Stage {testStage}");
            }
        }

        if (GUILayout.Button("Save All Game Data"))
        {
            userdata.SaveGameData();
            Debug.Log("Game data saved.");
        }

        if (GUILayout.Button("Load All Game Data"))
        {
            userdata.LoadGameData();
            Debug.Log($"Game data loaded. {userdata.GetCount()}");
            // Display catnip status for each stage
            for (int i = 1; i <= userdata.GetCount(); i++)
            {
                var stageData = userdata.GetStageData(i);
                if (stageData != null)
                {
                    Debug.Log($"Stage {i} Data - Best Time: {stageData.bestTime}, Catnip: {string.Join(", ", stageData.catnipCollected)}");
                }
                else
                {
                    Debug.Log($"No data found for Stage {i}");
                }
            }
            Debug.Log("Game data loaded.");
        }

        GUILayout.Space(10);
        GUILayout.Label("Debug Actions");
        if (GUILayout.Button("Reset All PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs reset. All saved data cleared.");
        }

        GUILayout.EndVertical();
    }
}