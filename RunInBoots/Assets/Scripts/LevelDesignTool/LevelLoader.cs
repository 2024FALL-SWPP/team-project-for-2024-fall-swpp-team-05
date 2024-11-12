using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            terrainData = JsonUtility.FromJson<TerrainData>(json);
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

        // 먼저 모든 파이프를 위치에 맞게 생성하고 리스트에 저장
        List<GameObject> createdPipes = new List<GameObject>();

        foreach (var objectPosition in terrainData.objectPositions.objPos)
        {
            string prefabName = objectPosition.name;
            GameObject prefab = Resources.Load<GameObject>("LevelObject/" + prefabName);

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab '{prefabName}'을 찾을 수 없습니다.");
                continue;
            }

            foreach (Vector3 position in objectPosition.positions)
            {
                Vector3 adjustedPosition = position - minPosition;
                GameObject instance = Instantiate(prefab, adjustedPosition, Quaternion.identity, parentTransform);
                Debug.Log($"Placing object '{prefabName}' at {adjustedPosition}");

                // 파이프 오브젝트를 발견하면 리스트에 저장
                if (prefabName == "Pipe")
                {
                    createdPipes.Add(instance);
                }
            }
        }

        // 파이프 데이터 처리: pipeData 리스트의 순서대로 각 파이프에 설정 정보 적용
        for (int i = 0; i < terrainData.pipeConnections.pipeList.Count; i++)
        {
            // 생성된 파이프 수와 pipeData 수가 일치하는지 확인
            if (i >= createdPipes.Count)
            {
                Debug.LogWarning("생성된 파이프의 수가 pipeData의 수보다 적습니다. 남은 pipeData는 적용되지 않습니다.");
                break;
            }

            PipeData pipeData = terrainData.pipeConnections.pipeList[i];
            GameObject pipeInstance = createdPipes[i];
            Pipe pipeComponent = pipeInstance.GetComponent<Pipe>();

            if (pipeComponent != null)
            {
                pipeComponent.pipeID = pipeData.pipeID;
                pipeComponent.targetPipeID = pipeData.targetPipeID;
                pipeComponent.targetStage = pipeData.targetStage;
                pipeComponent.targetIndex = pipeData.targetIndex;

                Debug.Log($"Pipe configured with ID: {pipeComponent.pipeID}, Target Pipe ID: {pipeComponent.targetPipeID}, Target Stage: {pipeComponent.targetStage}, Target Index: {pipeComponent.targetIndex}");
            }
            else
            {
                Debug.LogError("Pipe prefab에 Pipe 컴포넌트가 없습니다.");
            }
        }
    }

    private Vector3 FindMinPosition()
    {
        Vector3 minPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        foreach (var objectPosition in terrainData.objectPositions.objPos)
        {
            foreach (Vector3 position in objectPosition.positions)
            {
                minPosition = Vector3.Min(minPosition, position);
            }
        }
        return minPosition;
    }
}
