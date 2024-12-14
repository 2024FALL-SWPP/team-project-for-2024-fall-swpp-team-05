using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class OutGameManager : MonoBehaviour
{
    public Button goBackButton; // Assign via Inspector
    public Button gameStartButton; // Assign via Inspector
    public Button selectStageButton; // Assign via Inspector
    public Button exitButton; // Assign via Inspector
    public Image titleSceneImage; // Assign via Inspector
    public UserData userData; // Assign via Inspector

    public GameObject stageSelectUI; // Assign via Inspector
    public GameObject titleUI; // Assign via Inspector
    public Button[] stageButtonList;
    public TextMeshProUGUI[] stageRecordTextList;

    public Image[] catnipImageList; // Assign via Inspector

    void Start()
    {
        AudioManager.Instance.PlayAudio(0);
        
        goBackButton.onClick.RemoveAllListeners(); // Remove any old listeners
        goBackButton.onClick.AddListener(GoBackToTitleScene);
    
        
        userData = new UserData();
        userData.LoadGameData();
        Debug.Log($"Loaded game data: Lives={userData.lives}, RecentStage={userData.recentStage}, StageDataCount={userData.GetCount()}");

        gameStartButton.onClick.RemoveAllListeners(); // Remove any old listeners
        gameStartButton.onClick.AddListener(() => GameManager.Instance.StartNewStage(userData.recentStage));

        for (int i=0; i<5; i++) {
            stageButtonList[i].onClick.RemoveAllListeners(); // Remove any old listeners
        }
        stageButtonList[0].onClick.AddListener(() => GameManager.Instance.StartNewStage(1));
        stageButtonList[1].onClick.AddListener(() => GameManager.Instance.StartNewStage(2));
        stageButtonList[2].onClick.AddListener(() => GameManager.Instance.StartNewStage(3));
        stageButtonList[3].onClick.AddListener(() => GameManager.Instance.StartNewStage(4));
        stageButtonList[4].onClick.AddListener(() => GameManager.Instance.StartNewStage(5));

        // initialize catnip images
        for (int i = 0; i < catnipImageList.Length; i++)
        {
            catnipImageList[i].color = new Color(0,0,0,0);
        }
        Debug.Log(GameObject.FindGameObjectWithTag("HeartIconContainer").name);
        StageUIUtils.PlaceHeartIcons(userData.lives);
        stageSelectUI.SetActive(false);
    }

    void GoBackToTitleScene()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        titleUI.SetActive(true);
        stageSelectUI.SetActive(false);
    }

    public void ExitGameMode()
    {
#if UNITY_EDITOR
        // Only works in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Closes the application if in a build
        Application.Quit();
#endif
    }

    private int _GetCatnipIdx(int stage, int catnip)
    {
        return (stage - 1) * 3 + catnip;
    }

    public void LoadSelectStageScene()
    {
        // SceneManager.LoadScene("SelectStageScene");
        titleUI.SetActive(false);
        stageSelectUI.SetActive(true);


        for (int stage = 1; stage <= 5; stage++)
        {
            // assert(stageButton != null, "Child object of stageButtonList is not a Button");
            if (stage == 1) {
                stageButtonList[stage-1].interactable = true;
            }
            else {
                stageButtonList[stage-1].interactable = userData.IsUnlockedStage(stage);
            }
            StageData stageData = userData.GetStageData(stage);
            if (stageData == null) {
                stageRecordTextList[stage-1].text = "";
                Debug.Log($"Stage {stage} is not unlocked");
                continue;
            }
            int stageRecord = stageData.bestTime;
            List<bool> catnipStatus = stageData.catnipCollected;

            // set position of stageRecordTextList[stage-1] right below the button
            // stageRecordTextList[stage-1].transform.position = new Vector3(stageButtonList[stage-1].transform.position.x, stageButtonList[stage-1].transform.position.y - 50, stageButtonList[stage-1].transform.position.z);
            if (stageRecord > 0) {
                stageRecordTextList[stage-1].text = $"Record: {stageRecord}s";
            }
            else {
                stageRecordTextList[stage-1].text = "";
            }

            for (int catnip = 0; catnip < catnipStatus.Count; catnip++)
                catnipImageList[_GetCatnipIdx(stage, catnip)].color = catnipStatus[catnip] ? Color.white : new Color(0,0,0,0);
        }
    }
}