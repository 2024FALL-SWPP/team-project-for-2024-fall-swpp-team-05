using System;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class GridManager : MonoBehaviour
{
    public SerializableVector2Int gridSize;
    public GameObject gridPlane;
    public TerrainDataLoader terrainDataLoader;
    public Camera mainCamera;

    // 팔레트 UI 관련
    public ToggleGroup paletteToggleGroup;
    public Toggle paletteTogglePrefab;
    public RectTransform paletteContent;
    public GameObject palleteScroll;
    public GameObject gridControllPanel;
    public GameObject gridSizeXInputField;
    public GameObject gridSizeYInputField;

    private int gridSizeX;
    private int gridSizeY;

    public Vector3 dragStartWorldPos, dragEndWorldPos;

    // 현재 선택된 오브젝트
    private string selectedPrefabName = null;
    bool isGridMode = false;

    public Vector2 planePosOffset = new Vector2(0, 0);

    public float delayTime = 1f;

    public int numGrids = 0;

    public Material nonSelectedMaterial, selectedMaterial;

    private float startTime;

    // 배치된 오브젝트들
    private Dictionary<SerializableVector2Int, GameObject> placedObjects = new Dictionary<SerializableVector2Int, GameObject>();
    private List<List<GameObject>> placedSpheres = new List<List<GameObject>>();
    public float cameraPosZ = -10.0f;

    void Start()
    {
        terrainDataLoader = GameObject.FindObjectOfType<TerrainDataLoader>();
        palleteScroll.SetActive(false);
        gridControllPanel.SetActive(false);

        nonSelectedMaterial = Resources.Load<Material>("Materials/NonSelectedMaterial");
        selectedMaterial = Resources.Load<Material>("Materials/SelectedMaterial");
        nonSelectedMaterial.color = Color.black;
        selectedMaterial.color = Color.red;
    }

    public void CreateGrid()
    {

        terrainDataLoader = GameObject.FindObjectOfType<TerrainDataLoader>();
        if (gridSize.x == 0 || gridSize.y == 0) {
            gridSize = terrainDataLoader.terrainData.gridSize;
            Debug.Log("Grid size: " + gridSize.x + ", " + gridSize.y);
        }
        // 그리드 크기를 기반으로 Plane 생성 또는 Gizmos로 그리드 그리기
        // Plane의 크기를 gridSize에 맞게 조절
        // if old plane already exists, remove it
        if (gridPlane != null)
        {
            gridPlane.SetActive(false);
        }
        gridPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planePosOffset.x = gridSize.x / 2 - 10; // TODO: Full HD
        planePosOffset.y = gridSize.y / 2 - 5; // TODO: Full HD

        gridPlane.transform.position = new Vector3(planePosOffset.x, planePosOffset.y, 0);
        // rotation을 90도로 설정하여 Plane이 수평으로 생성되도록 함
        gridPlane.transform.rotation = Quaternion.Euler(90, 0, 0);
        gridPlane.transform.localScale = new Vector3(gridSize.x, 1, gridSize.y);

        gridPlane.GetComponent<Renderer>().material.color = Color.white;

        // initialize placedSpheres
        placedSpheres = new List<List<GameObject>>();

        // 각 cell의 중심 좌표에 sphere 생성
        for (int x = 0; x < gridSize.x; x++)
        {
            placedSpheres.Add(new List<GameObject>());
            for (int y = 0; y < gridSize.y; y++)
            {
                // gridPlane의 중심을 기준으로 x, y만큼 이동한 위치 계산
                Vector3 cellCenter = gridPlane.transform.position + new Vector3(x - gridSize.x / 2 + 0.5f, y - gridSize.y / 2 + 0.5f, 0);
                string cellName = "Cell_" + x + "_" + y;
                // in case scaling up grid size
                // if cell already exists, skip creating new cell
                GameObject cell = GameObject.Find(cellName);
                if (cell != null)
                {
                    placedSpheres[x].Add(cell);
                    continue;
                }
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = cellName;
                sphere.tag = "Cell";
                sphere.transform.position = cellCenter;
                sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                sphere.GetComponent<MeshRenderer>().material = nonSelectedMaterial;
                placedSpheres[x].Add(sphere);
            }
        }

        // print placedSpheres size
        Debug.Log("placedSpheres size: " + placedSpheres.Count + " x " + placedSpheres[0].Count);

        // remove all objects/cells outside of current gridSize
        List<SerializableVector2Int> keysToRemove = new List<SerializableVector2Int>();
        foreach (var entry in placedObjects)
        {
            SerializableVector2Int gridPos = entry.Key;
            if (gridPos.x < 0 || gridPos.x >= gridSize.x || gridPos.y < 0 || gridPos.y >= gridSize.y)
            {
                entry.Value.SetActive(false);
                keysToRemove.Add(gridPos);
            }
        }

        foreach (var key in keysToRemove)
        {
            placedObjects.Remove(key);
        }
        
        // remove all spheres outside of current gridSize
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Cell");
        foreach (GameObject obj in objects)
        {
            string[] cellName = obj.name.Split('_');
            int x = int.Parse(cellName[1]);
            int y = int.Parse(cellName[2]);
            if (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y)
            {
                obj.SetActive(false);
            }
        }
        
    }

    void OnDrawGizmos()
    {
        // Draw a colored sphere at the transform's position
        Gizmos.color = Color.black;
        //Debug.Log("Drawing gizmos at " + transform.position);
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    public void LoadPalette()
    {
        Debug.Log("Load Palette");
        palleteScroll.SetActive(true);
        gridControllPanel.SetActive(true);

        // Resources/LevelObject 폴더에서 프리팹 목록 로드
        GameObject[] prefabs = Resources.LoadAll<GameObject>("LevelObject");
        foreach (GameObject prefab in prefabs)
        {
            Toggle toggle = Instantiate(paletteTogglePrefab, paletteContent);
            toggle.group = paletteToggleGroup;
            toggle.GetComponentInChildren<Text>().text = prefab.name;
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    selectedPrefabName = prefab.name;
                }
                if (!isOn)
                {
                    selectedPrefabName = null;
                }
            });
        }
    }

    public void OnSetGridButtonClicked()
    {
        gridSizeX = int.Parse(gridSizeXInputField.GetComponentInChildren<TMP_InputField>().text);
        gridSizeY = int.Parse(gridSizeYInputField.GetComponentInChildren<TMP_InputField>().text);
        
        terrainDataLoader.terrainData.gridSize = new SerializableVector2Int(gridSizeX, gridSizeY);
        gridSize.x = gridSizeX;
        gridSize.y = gridSizeY;
        CreateGrid();
        Debug.Log($"Grid size set to: {terrainDataLoader.terrainData.gridSize.x} x {terrainDataLoader.terrainData.gridSize.y}");
    }

    public void StartGridMode()
    {
        isGridMode = true;
        startTime = Time.time;
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;

        if (elapsedTime >= delayTime && isGridMode && gridSize.x > 0 && gridSize.y > 0)
        {
            numGrids = gridSize.x * gridSize.y;
            HandleMouseInput();
        }
    }
    
    void HandleMouseInput()
    {
        // 마우스 좌클릭 드래그로 배치 범위 선택 및 오브젝트 배치
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            // 드래그 시작 지점 저장
            Debug.Log("Mouse Down");
            dragStartWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cameraPosZ));
            Debug.Log(dragStartWorldPos);
        }

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            // 드래그 중인 범위 내에 있는 cell 표시
            Vector3 currentWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cameraPosZ));
            Vector3 minWorldPos = new Vector3(Mathf.Min(dragStartWorldPos.x, currentWorldPos.x), Mathf.Min(dragStartWorldPos.y, currentWorldPos.y), 0);
            Vector3 maxWorldPos = new Vector3(Mathf.Max(dragStartWorldPos.x, currentWorldPos.x), Mathf.Max(dragStartWorldPos.y, currentWorldPos.y), 0);

            SerializableVector2Int minGridPos = WorldToGrid(minWorldPos);
            SerializableVector2Int maxGridPos = WorldToGrid(maxWorldPos);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (x >= minGridPos.x && x <= maxGridPos.x && y >= minGridPos.y && y <= maxGridPos.y)
                    {
                        GameObject sphere = placedSpheres[x][y];
                        sphere.GetComponent<MeshRenderer>().material = selectedMaterial;
                    }
                    else
                    {
                        GameObject sphere = placedSpheres[x][y];
                        sphere.GetComponent<MeshRenderer>().material = nonSelectedMaterial;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Debug.Log("Mouse Up");
            // 드래그 종료 지점 저장
            dragEndWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cameraPosZ));
            // 드래그 종료 지점 계산하여 범위 내 오브젝트 배치
            PlaceObjectsInRange();
        }

        // 마우스 우클릭 드래그로 카메라 이동
        if (Input.GetMouseButton(1))
        {
            float moveSpeed = 0.5f;
            float h = -Input.GetAxis("Mouse X") * moveSpeed;
            float v = -Input.GetAxis("Mouse Y") * moveSpeed;
            mainCamera.transform.Translate(new Vector3(h, v, 0));

            Vector3 cameraPos = mainCamera.transform.position;
            float halfGridWidth = gridSize.x / 2f;      //-8.0f
            float halfGridHeight = gridSize.y / 2f;

            cameraPos.x = Mathf.Clamp(cameraPos.x, planePosOffset.x - halfGridWidth, planePosOffset.x + halfGridWidth);
            cameraPos.y = Mathf.Clamp(cameraPos.y, planePosOffset.y - halfGridHeight, planePosOffset.y + halfGridHeight);

            mainCamera.transform.position = cameraPos;
        }
    }

    SerializableVector2Int WorldToGrid(Vector3 worldPos)
    {
        // 월드 좌표를 그리드 좌표로 변환
        int x = Mathf.RoundToInt(worldPos.x - planePosOffset.x + gridSize.x / 2 - 0.5f);
        int y = Mathf.RoundToInt(worldPos.y - planePosOffset.y + gridSize.y / 2 - 0.5f);
        return new SerializableVector2Int(x, y);
    }

    Vector3 GridToWorld(SerializableVector2Int gridPos)
    {
        // 그리드 좌표를 월드 좌표로 변환
        float x = gridPos.x + planePosOffset.x - gridSize.x / 2 + 0.5f;
        float y = gridPos.y + planePosOffset.y - gridSize.y / 2 + 0.5f;
        return new Vector3(x, y, 0);
    }

    public void SaveGridData()
    {
        // save grid data to terrainDataLoader
        //Debug.Log("Save grid data");
        terrainDataLoader.terrainData.levelObjects.Clear();
        terrainDataLoader.terrainData.gridSize = gridSize;

        int catnipCount = 0;

        foreach (var entry in placedObjects)
        {
            SerializableVector2Int gridPos = entry.Key;
            GameObject obj = entry.Value;
            string prefabName = obj.name.Replace("(Clone)", "");

            LevelObjectData data = null;
            if (prefabName == "Pipe")
            {
                PipeData pipeData = new PipeData
                {
                    pipeID = obj.GetComponent<Pipe>().pipeID,
                    targetPipeID = obj.GetComponent<Pipe>().targetPipeID,
                    targetIndex = obj.GetComponent<Pipe>().targetIndex
                };
                data = pipeData;
            }
            else if (prefabName == "StartPoint")
            {
                data = new StartPointData();
            }
            else if (prefabName == "GoalPoint")
            {
                data = new GoalPointData();
            }
            else if (prefabName == "Catnip")
            {
                catnipCount++;
                CatnipData catnipData = new CatnipData
                {
                    catnipID = obj.GetComponent<Catnip>().catnipID
                };
                data = catnipData;
            }
            else
            {
                data = new LevelObjectData();
            }

            data.gridPosition = gridPos;
            data.objectType = prefabName;

            terrainDataLoader.terrainData.levelObjects.Add(data);
        }
        terrainDataLoader.terrainData.catnipCount = catnipCount;
    }

    public void LoadGridData()
    {
        // load grid data from terrainDataLoader
        Debug.Log("Load grid data");
        placedObjects.Clear();
        foreach (var levelObjectData in terrainDataLoader.terrainData.levelObjects)
        {
            string prefabName = levelObjectData.objectType;
            GameObject prefab = Resources.Load<GameObject>("LevelObject/" + prefabName);
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab '{prefabName}'을 찾을 수 없습니다.");
                continue;
            }

            Vector3 position = GridToWorld(levelObjectData.gridPosition);
            GameObject obj = PoolManager.Instance.Pool(prefab, position, Quaternion.identity);
            placedObjects.Add(levelObjectData.gridPosition, obj);

            // 생성된 오브젝트에 데이터 할당
            if (levelObjectData is PipeData pipeData)
            {
                Pipe pipeComponent = obj.GetComponent<Pipe>();
                if (pipeComponent != null)
                {
                    pipeComponent.pipeID = pipeData.pipeID;
                    pipeComponent.targetPipeID = pipeData.targetPipeID;
                    pipeComponent.targetIndex = pipeData.targetIndex;
                }
            }
            else if (levelObjectData is CatnipData catnipData)
            {
                Catnip catnipComponent = obj.GetComponent<Catnip>();
                if (catnipComponent != null)
                {
                    catnipComponent.catnipID = catnipData.catnipID;
                }
            }
        }
    
    }
    
    void PlaceObjectsInRange()
    {
        // 드래그된 범위 내의 그리드 좌표 계산
        // 이미 배치된 칸은 생략하고, 선택된 오브젝트로 채움
        // 선택된 오브젝트가 없으면 해당 범위의 오브젝트를 삭제(지우개 기능)
        Vector3 startWorldPos = dragStartWorldPos;
        Vector3 endWorldPos = dragEndWorldPos;
        Vector3 minWorldPos = new Vector3(Mathf.Min(startWorldPos.x, endWorldPos.x), Mathf.Min(startWorldPos.y, endWorldPos.y), 0);
        Vector3 maxWorldPos = new Vector3(Mathf.Max(startWorldPos.x, endWorldPos.x), Mathf.Max(startWorldPos.y, endWorldPos.y), 0);

        SerializableVector2Int minGridPos = WorldToGrid(minWorldPos);
        SerializableVector2Int maxGridPos = WorldToGrid(maxWorldPos);

        for (int x = minGridPos.x; x <= maxGridPos.x; x++)
        {
            for (int y = minGridPos.y; y <= maxGridPos.y; y++)
            {
                SerializableVector2Int gridPos = new SerializableVector2Int(x, y);

                // erase object if selectedPrefabName is empty
                if (!string.IsNullOrEmpty(selectedPrefabName))
                {
                    if (placedObjects.ContainsKey(gridPos))
                    {
                        continue;
                    }
                    // grid bounds 내에 있는지 확인
                    if (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y)
                    {
                        continue;
                    }
                    GameObject prefab = Resources.Load<GameObject>("LevelObject/" + selectedPrefabName);
                    GameObject obj = PoolManager.Instance.Pool(prefab, GridToWorld(gridPos), Quaternion.identity);
                    placedObjects.Add(gridPos, obj);

                    LevelObjectData data = null;

                    if (selectedPrefabName == "Pipe")
                    {
                        PipeData pipeData = new PipeData();
                        pipeData.pipeID = obj.GetComponent<Pipe>().pipeID;
                        pipeData.targetPipeID = obj.GetComponent<Pipe>().targetPipeID;
                        pipeData.targetIndex = obj.GetComponent<Pipe>().targetIndex;
                        data = pipeData;
                    }
                    else if (selectedPrefabName == "StartPoint")
                    {
                        data = new StartPointData();
                    }
                    else if (selectedPrefabName == "GoalPoint")
                    {
                        data = new GoalPointData();
                    }
                    else if (selectedPrefabName == "Catnip")
                    {
                        CatnipData catnipData = new CatnipData();
                        catnipData.catnipID = obj.GetComponent<Catnip>().catnipID;
                        data = catnipData;
                    }
                    else
                    {
                        data = new LevelObjectData();
                    }

                    data.gridPosition = gridPos;
                    data.objectType = selectedPrefabName;

                    terrainDataLoader.terrainData.levelObjects.Add(data);
                }
                else
                {
                    if (placedObjects.ContainsKey(gridPos))
                    {
                        placedObjects[gridPos].SetActive(false);
                        placedObjects.Remove(gridPos);

                        terrainDataLoader.terrainData.levelObjects.RemoveAll(data => data.gridPosition.Equals(gridPos));
                    }
                }
            }
        }
    }
}
