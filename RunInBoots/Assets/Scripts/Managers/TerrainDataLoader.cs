using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using TMPro;


public class TerrainDataLoader : MonoBehaviour
{
    public string stage;
    public TextMeshProUGUI stageInputField, indexInputField;
    public string terrainIndex;
    public string fileName;
    public TerrainData terrainData;

    public UIManager uiManager;
    public GridManager gridManager;

    // Singleton instance
    public static TerrainDataLoader Instance;

    public void Start()
    {
        uiManager = GameObject.FindObjectOfType<UIManager>();
        gridManager = GameObject.FindObjectOfType<GridManager>();
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
        gridManager.SaveGridData();
        string json = JsonUtility.ToJson(terrainData, true); // prettyPrint 옵션을 true로 설정하여 가독성을 높임
        string path = Application.dataPath + "/Resources/TerrainData/" + fileName + ".json";
        File.WriteAllText(path, json);
        Debug.Log("Level data saved to " + path);
    }

    public void LoadTerrainData()
    {
        stage = stageInputField.text;
        terrainIndex = indexInputField.text;
        terrainData.stage = stage;
        terrainData.terrainIndex = terrainIndex;
        fileName = $"Stage_{stage}_{terrainIndex}";
        Debug.Log("Loading terrain data for " + fileName);
        string path = Application.dataPath + "/Resources/TerrainData/" + fileName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            terrainData = JsonUtility.FromJson<TerrainData>(json);
            Debug.Log("Level data loaded from " + path);
            LoadTerrain();
        }
        else
        {
            Debug.Log("No level data found at " + path);
            CreateTerrain();
            
        }

        uiManager.InstantiateTerrain();
        gridManager.StartGridMode();
    }


    void LoadTerrain()
    {
        GameObject levelParent = new GameObject("Level");
        GameObject blocksParent = new GameObject("Blocks");

        gridManager.LoadGridData();
        gridManager.CreateGrid();

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
        terrainData.gridSize = new SerializableVector2Int(10, 10);
        Debug.Log($"New terrain data created for {fileName}");
        gridManager.CreateGrid();
    }
}
