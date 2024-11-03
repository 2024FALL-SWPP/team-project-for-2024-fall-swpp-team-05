using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    
    public Vector3 dragStartWorldPos, dragEndWorldPos;

    // 현재 선택된 오브젝트
    string selectedPrefabName = "Block";
    bool isGridMode = false;

    // 배치된 오브젝트들
    private Dictionary<SerializableVector2Int, GameObject> placedObjects = new Dictionary<SerializableVector2Int, GameObject>();
    public float cameraPosZ = -10.0f;

    void Start()
    {
        // 팔레트 생성
        // LoadPalette();
    }

    public void CreateGrid()
    {
        terrainDataLoader = GameObject.FindObjectOfType<TerrainDataLoader>();
        gridSize = terrainDataLoader.terrainData.gridSize;
        Debug.Log("Grid size: " + gridSize.x + ", " + gridSize.y);
        // 그리드 크기를 기반으로 Plane 생성 또는 Gizmos로 그리드 그리기
        // Plane의 크기를 gridSize에 맞게 조절
        gridPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gridPlane.transform.position = new Vector3(0, 0, 0);
        // rotation을 90도로 설정하여 Plane이 수평으로 생성되도록 함
        gridPlane.transform.rotation = Quaternion.Euler(90, 0, 0);
        gridPlane.transform.localScale = new Vector3(gridSize.x, 1, gridSize.y);

        gridPlane.GetComponent<Renderer>().material.color = Color.white;

        // 각 cell의 중심 좌표에 sphere 생성
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // gridPlane의 중심을 기준으로 x, y만큼 이동한 위치 계산
                Vector3 cellCenter = gridPlane.transform.position + new Vector3(x - gridSize.x / 2 + 0.5f, y - gridSize.y / 2 + 0.5f, 0);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "Cell_" + x + "_" + y;
                sphere.transform.position = cellCenter;
                sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                sphere.GetComponent<Renderer>().material.color = Color.black;
            }
        }
    }
        

    void LoadPalette()
    {
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
            });
        }
    }

    public void StartGridMode()
    {
        // 그리드 모드 시작
        // 팔레트 활성화
        isGridMode = true;
    }

    void Update()
    {
        if (isGridMode && gridSize.x > 0 && gridSize.y > 0)
        {
            HandleMouseInput();
        }
    }

    void HandleMouseInput()
    {
        // 마우스 좌클릭 드래그로 배치 범위 선택 및 오브젝트 배치
        if (Input.GetMouseButtonDown(0))
        {
            // 드래그 시작 지점 저장
            Debug.Log("Mouse Down");
            dragStartWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cameraPosZ));
            Debug.Log(dragStartWorldPos);
        }

        if (Input.GetMouseButton(0))
        {
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
                        GameObject sphere = GameObject.Find("Cell_" + x + "_" + y);
                        sphere.GetComponent<Renderer>().material.color = Color.red;
                    }
                    else {
                        GameObject sphere = GameObject.Find("Cell_" + x + "_" + y);
                        sphere.GetComponent<Renderer>().material.color = Color.black;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Up");
            // 드래그 종료 지점 저장
            dragEndWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cameraPosZ));
            // 드래그 종료 지점 계산하여 범위 내 오브젝트 배치
            PlaceObjectsInRange();
        }

        // 마우스 우클릭 드래그로 카메라 이동
        if (Input.GetMouseButton(1))
        {
            float moveSpeed = 0.1f;
            float h = -Input.GetAxis("Mouse X") * moveSpeed;
            float v = -Input.GetAxis("Mouse Y") * moveSpeed;
            mainCamera.transform.Translate(new Vector3(h, v, 0));
        }
    }

    SerializableVector2Int WorldToGrid(Vector3 worldPos)
    {
        // 월드 좌표를 그리드 좌표로 변환
        int x = Mathf.RoundToInt(worldPos.x + gridSize.x / 2 - 0.5f);
        int y = Mathf.RoundToInt(worldPos.y + gridSize.y / 2 - 0.5f);
        return new SerializableVector2Int(x, y);
    }

    Vector3 GridToWorld(SerializableVector2Int gridPos)
    {
        // 그리드 좌표를 월드 좌표로 변환
        float x = gridPos.x - gridSize.x / 2 + 0.5f;
        float y = gridPos.y - gridSize.y / 2 + 0.5f;
        return new Vector3(x, y, 0);
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
                if (placedObjects.ContainsKey(gridPos))
                {
                    continue;
                }

                if (selectedPrefabName != null)
                {
                    // grid bounds 내에 있는지 확인
                    if (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y)
                    {
                        continue;
                    }
                    GameObject prefab = Resources.Load<GameObject>("LevelObject/" + selectedPrefabName);
                    GameObject obj = Instantiate(prefab, GridToWorld(gridPos), Quaternion.identity);
                    placedObjects.Add(gridPos, obj);
                }
                else
                {
                    if (placedObjects.ContainsKey(gridPos))
                    {
                        Destroy(placedObjects[gridPos]);
                        placedObjects.Remove(gridPos);
                    }
                }
            }
        }
    }
}
