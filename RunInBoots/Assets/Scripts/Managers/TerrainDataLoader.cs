using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using TMPro;


public class TerrainDataLoader : MonoBehaviour
{
    public GameObject stageInputField;
    public GameObject indexInputField;
    public GameObject loadCanvas;
    public TerrainData terrainData;

    private string stage;
    private string terrainIndex;
    private string fileName;
    
    

    // Singleton instance
    public static TerrainDataLoader Instance;

    void Awake()
    {
        // Singleton 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            // 필요에 따라 씬 전환 시에도 파괴되지 않도록 설정
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // public void SaveTerrainData()
    // {
    //     string path = Application.dataPath + "/Resources/TerrainData/" + fileName + ".dat";
    //     FileStream file = File.Create(path);
    //     BinaryFormatter bf = new BinaryFormatter();
    //     bf.Serialize(file, terrainData);
    //     file.Close();
    //     Debug.Log("Terrain data saved to " + path);
    // }

    public void SaveTerrainData()
    {
        string json = JsonUtility.ToJson(terrainData, true); // prettyPrint 옵션을 true로 설정하여 가독성을 높임
        string path = Application.dataPath + "/Resources/TerrainData/" + fileName + ".json";
        File.WriteAllText(path, json);
        Debug.Log("Level data saved to " + path);
    }

    // public void LoadTerrainData()
    // {
    //     string path = Application.dataPath + "/Resources/TerrainData/" + fileName + ".dat";
    //     if (File.Exists(path))
    //     {
    //         FileStream file = File.Open(path, FileMode.Open);
    //         BinaryFormatter bf = new BinaryFormatter();
    //         terrainData = (TerrainData)bf.Deserialize(file);
    //         file.Close();
    //         Debug.Log("Terrain data loaded from " + path);
    //         InstantiateTerrain();
    //     }
    //     else
    //     {
    //         Debug.Log("No terrain data found at " + path);
    //         stage = stageInputField.text;
    //         terrainIndex = indexInputField.text;
    //         // 새로운 TerrainData 생성
    //         terrainData = new TerrainData();
    //         terrainData.stage = stage;
    //         terrainData.terrainIndex = terrainIndex;
    //         terrainData.gridSize = new Vector2Int(200, 80);
    //         fileName = $"Stage_{stage}_{terrainIndex}";
    //         Debug.Log($"New terrain data created for {fileName}");
    //     }
    // }

    public void LoadTerrainData()
    {
        string path = Application.dataPath + "/Resources/TerrainData/" + fileName + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            terrainData = JsonUtility.FromJson<TerrainData>(json);
            Debug.Log("Level data loaded from " + path);
            InstantiateTerrain();
        }
        else
        {
            Debug.Log("No level data found at " + path);
            stage = stageInputField.GetComponentInChildren<TMP_InputField>().text;
            terrainIndex = indexInputField.GetComponentInChildren<TMP_InputField>().text;
            // 새로운 terrainData 생성
            terrainData = new TerrainData();
            terrainData.stage = stage;
            terrainData.terrainIndex = terrainIndex;
            terrainData.gridSize = new SerializableVector2Int(200, 80);
            fileName = $"Stage_{stage}_{terrainIndex}";
            Debug.Log($"New terrain data created for {fileName}");
        }

        loadCanvas.SetActive(false);
    }


    void InstantiateTerrain()
    {
        GameObject levelParent = new GameObject("Level");
        GameObject blocksParent = new GameObject("Blocks");
        blocksParent.transform.parent = levelParent.transform;

        foreach (var entry in terrainData.objectPositions)
        {
            string prefabName = entry.Key;
            List<Vector3> positions = entry.Value;
            GameObject prefab = Resources.Load<GameObject>("LevelObject/" + prefabName);
            foreach (Vector3 pos in positions)
            {
                GameObject obj = Instantiate(prefab, pos, Quaternion.identity, blocksParent.transform);
                // 필요한 경우 추가 설정
            }
        }

        // CameraConfiner 설정
        GameObject cameraConfiner = new GameObject("CameraConfiner");
        cameraConfiner.transform.parent = levelParent.transform;
        BoxCollider confinerCollider = cameraConfiner.AddComponent<BoxCollider>();
        confinerCollider.isTrigger = true;

        // 배치된 오브젝트들의 범위 계산
        // ...

        // 카메라에 Confiner 설정
        // CinemachineConfiner 등의 컴포넌트 추가 및 설정
    }
}
