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
            string fileName = $"Stage_{stage}_{index}.json";
            string path = Path.Combine(Application.dataPath, "Resources", "TerrainData", fileName);

            if (!File.Exists(path))
            {
                Debug.Log($"파일 {fileName}을 찾을 수 없습니다. 종료합니다.");
                break;
            }

            string json = File.ReadAllText(path);
            TerrainData terrainData = JsonConvert.DeserializeObject<TerrainData>(json);

            totalCatnip += terrainData.catnipCount; // 캣닢 개수 누적
        }

        Debug.Log($"Stage {stage}의 총 캣닢 개수: {totalCatnip}");
        return totalCatnip;
    }
}
