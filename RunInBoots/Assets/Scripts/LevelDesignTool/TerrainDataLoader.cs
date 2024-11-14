using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using TMPro;
using Newtonsoft.Json;


public class TerrainDataLoader : MonoBehaviour
{
    public GridManager gridManager;
    
    public GameObject stageInputField;
    public GameObject indexInputField;
    public GameObject loadPanel;
    public TerrainData terrainData;

    private string stage;
    public string terrainIndex;
    private string fileName;
    private string path;
    
    // Singleton instance
    public static TerrainDataLoader Instance;

    private void Start()
    {
        loadPanel.SetActive(true);
    }


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

    public void SaveTerrainData()
    {
        terrainData.stage = stage;
        terrainData.terrainIndex = terrainIndex;
        gridManager.SaveGridData();

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        };

        string json = JsonConvert.SerializeObject(terrainData, settings); // prettyPrint 옵션을 true로 설정하여 가독성을 높임
        File.WriteAllText(path, json);
        Debug.Log("Level data saved to " + path);
    }

    public void LoadTerrainData()
    {
        stage = stageInputField.GetComponentInChildren<TMP_InputField>().text;
        terrainIndex = indexInputField.GetComponentInChildren<TMP_InputField>().text;
        fileName = $"Stage_{stage}_{terrainIndex}";
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
            LoadTerrain();
        }
        else
        {
            Debug.Log("No level data found at " + path);
            CreateTerrain();
        }

        loadPanel.SetActive(false);
        gridManager.LoadPalette();
        gridManager.CreateGrid();
        gridManager.StartGridMode();
    }


    void LoadTerrain()
    {
        GameObject levelParent = new GameObject("Level");
        GameObject blocksParent = new GameObject("Blocks");

        gridManager.CreateGrid();
        gridManager.LoadGridData();

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

    void CreateTerrain() {
        terrainData = new TerrainData();
        terrainData.gridSize = new SerializableVector2Int(20, 10);
        Debug.Log($"New terrain data created for {fileName}");
        gridManager.CreateGrid();
    }
}
