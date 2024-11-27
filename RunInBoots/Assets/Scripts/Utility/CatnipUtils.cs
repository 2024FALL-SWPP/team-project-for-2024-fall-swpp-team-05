using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CatnipUtils
{
    public static void InitializeCatnipCollectionStates(List<bool> isCatnipCollected, int count)
    {
        isCatnipCollected.Clear();
        for (int i = 0; i < count; i++)
        {
            isCatnipCollected.Add(false);
        }
    }

    public static void UpdateCatnipToCollected(List<bool> isCatnipCollected, int catnipID)
    {
        if (catnipID > 0 && catnipID <= isCatnipCollected.Count)
        {
            isCatnipCollected[catnipID - 1] = true;
        }
    }

    public static void CatnipCollectUIUpdate(int catnipID)
    {
        UIManager.Instance.UpdateCatnipUI(catnipID);
    }

    public static int CountTotalCatnipInStage(int stage)
    {
        int totalCatnip = 0;

        for (int index = 1; ; index++)
        {
            string fileName = $"Stage_{stage}_{index}";
            string path = Path.Combine("TerrainData", fileName);
            var file = Resources.Load<TextAsset>(path);

            if (file == null)
            {
                Debug.Log($"���� {fileName}�� ã�� �� �����ϴ�. �����մϴ�.");
                break;
            }

            string json = file.text;
            TerrainData terrainData = JsonConvert.DeserializeObject<TerrainData>(json);

            totalCatnip += terrainData.catnipCount; // Ĺ�� ���� ����
        }

        Debug.Log($"Stage {stage}�� �� Ĺ�� ����: {totalCatnip}");
        return totalCatnip;
    }
}
