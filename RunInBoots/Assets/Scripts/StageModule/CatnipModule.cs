using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections.ObjectModel;


public class CatnipModule
{
    private int _totalCatnipCount;
    private List<GameObject> _catnipIcons = new List<GameObject>();
    private List<bool> _isCatnipCollected = new List<bool>();

    public CatnipModule(int currentStage) 
    {
        InitializeCatnipInfo(currentStage);
        PlaceCatnipIcons();
    }

    private void InitializeCatnipInfo(int currentStage)
    {
        _totalCatnipCount = CountTotalCatnipInStage(currentStage);
        _isCatnipCollected = new List<bool>(new bool[_totalCatnipCount]);
    }

    public IReadOnlyList<bool> GetIsCatnipCollected() => _isCatnipCollected;

    //public List<bool> GetIsCatnipCollected() => new ReadOnlyCollection<bool>(_isCatnipCollected);

    public void ClearCatnipIcons() => _catnipIcons.Clear();

    public void PlaceCatnipIcons()
    {
        StageUIUtils.PlaceCatnipIcons(_totalCatnipCount, _isCatnipCollected, ref _catnipIcons);
    }
    
    private int CountTotalCatnipInStage(int stage)
    {
        int totalCatnip = 0;

        for (int index = 1; ; index++)
        {
            string fileName = $"Stage_{stage}_{index}";
            string path = Path.Combine("TerrainData", fileName);
            var file = Resources.Load<TextAsset>(path);

            if (file == null)
            {
                Debug.Log($"파일 {fileName}을 찾을 수 없습니다. 종료합니다.");
                break;
            }

            string json = file.text;
            TerrainData terrainData = JsonConvert.DeserializeObject<TerrainData>(json);

            totalCatnip += terrainData.catnipCount; // 캣닢 개수 누적
        }

        Debug.Log($"Stage {stage}의 총 캣닢 개수: {totalCatnip}");
        return totalCatnip;
    }


    public void UpdateCatnipStateToCollected(int catnipID)
    {
        if (catnipID > 0 && catnipID <= _catnipIcons.Count)
        {
            _isCatnipCollected[catnipID - 1] = true;
            StageUIUtils.SetCatnipIconState(_catnipIcons[catnipID - 1], true);
        }
        else
        {
            Debug.LogWarning("�������� catnipID: " + catnipID);
        }
    }
}
