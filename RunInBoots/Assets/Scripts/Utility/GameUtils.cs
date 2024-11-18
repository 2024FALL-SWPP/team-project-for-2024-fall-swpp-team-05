using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtils
{
    public static Pipe FindPipeByID(int id)
    {
        Pipe[] pipes = GameObject.FindObjectsOfType<Pipe>();
        foreach (Pipe pipe in pipes)
        {
            if (pipe.pipeID == id)
            {
                return pipe;
            }
        }
        return null;
    }

    public static int CountTotalCatnipInStage(int stage)
    {
        int totalCatnip = 0;

        for (int index = 1; ; index++)
        {
            string fileName = $"Stage_{stage}_{index}";
            string path = Path.Combine("TerrainData", fileName);
            var file = Resources.Load<TextAsset>(path);

            if (file==null)
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
