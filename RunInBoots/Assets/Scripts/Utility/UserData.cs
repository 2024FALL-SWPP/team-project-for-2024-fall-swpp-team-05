using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public int bestTime; // 최고 기록
    public List<bool> catnipCollected; // 캣닢 획득 여부
}

public class UserData
{
    // 글로벌 데이터
    public int lives; // 목숨
    public int recentStage; // 최근 스테이지

    // 스테이지 데이터
    private Dictionary<int, StageData> stageData = new Dictionary<int, StageData>();

    public void SaveGameData()
    {
        PlayerPrefs.SetInt("Lives", lives);
        PlayerPrefs.SetInt("RecentStage", recentStage);
        Debug.Log($"Saved game data: Lives={lives}, RecentStage={recentStage}");

        foreach (var stage in stageData)
        {
            string key = $"Stage_{stage.Key}_Data";
            string json = JsonUtility.ToJson(stage.Value);
            PlayerPrefs.SetString(key, json);
        }

        PlayerPrefs.Save();
    }

    public void LoadGameData()
    {
        // 글로벌 데이터 로드
        if (PlayerPrefs.HasKey("Lives"))
        {
            lives = PlayerPrefs.GetInt("Lives");
        }
        else
        {
            lives = 9; // 기본값 3
        }
        

        if (PlayerPrefs.HasKey("RecentStage"))
        {
            recentStage = PlayerPrefs.GetInt("RecentStage");
        }
        else
        {
            recentStage = 1; // 기본값 1
        }

        // 스테이지 데이터 로드
        stageData.Clear();
        int stageCount = 1;
        while (PlayerPrefs.HasKey($"Stage_{stageCount}_Data"))
        {
            string key = $"Stage_{stageCount}_Data";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                StageData data = JsonUtility.FromJson<StageData>(json);
                stageData[stageCount] = data;
            }
            stageCount++;
        }
    }

    public void SaveStageData(int stageNumber, int timeTaken, List<bool> catnipStatus)
    {
        if (!stageData.ContainsKey(stageNumber))
        {
            Debug.Log($"Saving new data for stage {stageNumber}");
            stageData[stageNumber] = new StageData
            {
                bestTime = timeTaken,
                catnipCollected = new List<bool>(catnipStatus)
            };
        }
        else
        {
            // 업데이트: 더 나은 기록이면 저장
            var data = stageData[stageNumber];
            if (timeTaken < data.bestTime || data.bestTime == 0)
            {
                data.bestTime = timeTaken;
            }

            // 캣닢 획득 상태 업데이트
            for (int i = 0; i < catnipStatus.Count; i++)
            {
                if (i < data.catnipCollected.Count)
                {
                    data.catnipCollected[i] |= catnipStatus[i];
                }
                else
                {
                    data.catnipCollected.Add(catnipStatus[i]);
                }
            }
        }

        // 저장
        SaveGameData();
    }

    public StageData GetStageData(int stageNumber)
    {
        return stageData.ContainsKey(stageNumber) ? stageData[stageNumber] : null;
    }

    public void UpdateLives(int newLives)
    {
        lives = newLives;
        SaveGameData();
    }

    public void UpdateRecentStage(int stageNumber)
    {
        recentStage = stageNumber;
        SaveGameData();
    }

    public int GetCount() {
        return stageData.Count;
    }
}