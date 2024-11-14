using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using Newtonsoft.Json;

public class LevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public int stage;
    public int index;


    private string fileName;
    private string path;
    public TerrainData terrainData;

    private void Start()
    {

    }

    public void LoadLevel()
    {
        fileName = $"Stage_{stage}_{index}";
        path = Application.dataPath + "/Resources/TerrainData/" + fileName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            terrainData = JsonConvert.DeserializeObject<TerrainData>(json, settings);
            Debug.Log("Level data loaded from " + path);

            CreateObjectsFromData();
        }
        else
        {
            Debug.LogError("No terrain data fount at " + path);
        }
    }

    private void CreateObjectsFromData()
    {
        Transform parentTransform = this.transform;
        Vector3 minPosition = FindMinPosition();

        foreach (var levelObjectData in terrainData.levelObjects)
        {
            string prefabName = levelObjectData.objectType;
            GameObject prefab = Resources.Load<GameObject>("LevelObject/" + prefabName);

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab '{prefabName}'을 찾을 수 없습니다.");
                continue;
            }

            Vector3 adjustedPosition = GridToWorldPosition(levelObjectData.gridPosition) - minPosition;
            GameObject instance = Instantiate(prefab, adjustedPosition, Quaternion.identity, parentTransform);
            //Debug.Log($"Placing object '{prefabName}' at {adjustedPosition}");
            if (levelObjectData is PipeData pipeData)
            {
                Debug.Log("$$$$$$");
                Pipe pipeComponent = instance.GetComponent<Pipe>();
                if (pipeComponent != null)
                {
                    Debug.Log("#####");
                    pipeComponent.pipeID = pipeData.pipeID;
                    pipeComponent.targetPipeID = pipeData.targetPipeID;
                    pipeComponent.targetStage = pipeData.targetStage;
                    pipeComponent.targetIndex = pipeData.targetIndex;
                }
                else
                {
                    Debug.LogError("No Pipe Script in Pipe");
                }
            }
            else if (levelObjectData is GoalPointData goalData)
            {
                GoalPoint goalComponent = instance.GetComponent<GoalPoint>();
                if (goalComponent != null)
                {
                    goalComponent.targetStage = goalData.targetStage;
                    goalComponent.targetIndex = goalData.targetIndex;
                }
            }
            else if (levelObjectData is CatnipData catnipData)
            {
                Catnip catnipComponent = instance.GetComponent<Catnip>();
                if (catnipComponent != null)
                {
                    catnipComponent.catnipID = catnipData.catnipID;
                }
            }
        }
    }

    private Vector3 FindMinPosition()
    {
        Vector3 minPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        foreach (var levelObjectData in terrainData.levelObjects)
        {
            Vector3 position = GridToWorldPosition(levelObjectData.gridPosition);
            minPosition = Vector3.Min(minPosition, position);
        }
        return minPosition;
    }

    private Vector3 GridToWorldPosition(SerializableVector2Int gridPosition)
    {
        // 그리드 좌표를 월드 좌표로 변환하는 방법을 정의합니다.
        // 예시로 1:1 비율을 사용할 수 있습니다.
        return new Vector3(gridPosition.x, gridPosition.y, 0);
    }
}
