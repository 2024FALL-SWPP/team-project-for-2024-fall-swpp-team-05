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

        // ���� ��� �������� ��ġ�� �°� �����ϰ� ����Ʈ�� ����
        List<GameObject> createdPipes = new List<GameObject>();

        foreach (var objectPosition in terrainData.objectPositions.objPos)
        {
            string prefabName = objectPosition.name;
            GameObject prefab = Resources.Load<GameObject>("LevelObject/" + prefabName);

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab '{prefabName}'�� ã�� �� �����ϴ�.");
                continue;
            }

            foreach (Vector3 position in objectPosition.positions)
            {
                Vector3 adjustedPosition = position - minPosition;
                GameObject instance = Instantiate(prefab, adjustedPosition, Quaternion.identity, parentTransform);
                Debug.Log($"Placing object '{prefabName}' at {adjustedPosition}");

                // ������ ������Ʈ�� �߰��ϸ� ����Ʈ�� ����
                if (prefabName == "Pipe")
                {
                    createdPipes.Add(instance);
                }
            }
        }

        // ������ ������ ó��: pipeData ����Ʈ�� ������� �� �������� ���� ���� ����
        for (int i = 0; i < terrainData.pipeConnections.pipeList.Count; i++)
        {
            // ������ ������ ���� pipeData ���� ��ġ�ϴ��� Ȯ��
            if (i >= createdPipes.Count)
            {
                Debug.LogWarning("������ �������� ���� pipeData�� ������ �����ϴ�. ���� pipeData�� ������� �ʽ��ϴ�.");
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
                Debug.LogError("Pipe prefab�� Pipe ������Ʈ�� �����ϴ�.");
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
