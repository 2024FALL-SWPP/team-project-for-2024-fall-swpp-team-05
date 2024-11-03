using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

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

    // 현재 선택된 오브젝트
    private string selectedPrefabName = null;

    // 배치된 오브젝트들
    private Dictionary<SerializableVector2Int, GameObject> placedObjects = new Dictionary<SerializableVector2Int, GameObject>();

    void Start()
    {
        terrainDataLoader = GameObject.FindObjectOfType<TerrainDataLoader>();
        // 그리드 생성
        CreateGrid();

        palleteScroll.SetActive(false);
        gridControllPanel.SetActive(false);
    }

    public void CreateGrid()
    {
        
        gridSize = terrainDataLoader.terrainData.gridSize;
        Debug.Log("Grid size: " + gridSize.x + ", " + gridSize.y);
        // 그리드 크기를 기반으로 Plane 생성 또는 Gizmos로 그리드 그리기
        // Plane의 크기를 gridSize에 맞게 조절
        gridPlane.transform.localScale = new Vector3(gridSize.x, 1, gridSize.y);

        // draw gizmos for each grid cell
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 cellCenter = new Vector3(x + 0.5f, 0, y + 0.5f);
                // TODO
                // transform.position = cellCenter;
                // OnDrawGizmos();
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
            });
        }
    }

    public void OnSetGridButtonClicked()
    {
        gridSizeX = int.Parse(gridSizeXInputField.GetComponentInChildren<TMP_InputField>().text);
        gridSizeY = int.Parse(gridSizeYInputField.GetComponentInChildren<TMP_InputField>().text);
        
        terrainDataLoader.terrainData.gridSize = new SerializableVector2Int(gridSizeX, gridSizeY);

        Debug.Log($"Grid size set to: {terrainDataLoader.terrainData.gridSize.x} x {terrainDataLoader.terrainData.gridSize.y}");
    }

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        // 마우스 좌클릭 드래그로 배치 범위 선택 및 오브젝트 배치
        if (Input.GetMouseButtonDown(0))
        {
            // 드래그 시작 지점 저장

        }

        if (Input.GetMouseButton(0))
        {
            // 드래그 중 범위 표시

        }

        if (Input.GetMouseButtonUp(0))
        {
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

    void PlaceObjectsInRange()
    {
        // 드래그된 범위 내의 그리드 좌표 계산
        // 이미 배치된 칸은 생략하고, 선택된 오브젝트로 채움
        // 선택된 오브젝트가 없으면 해당 범위의 오브젝트를 삭제(지우개 기능)
    }
}
