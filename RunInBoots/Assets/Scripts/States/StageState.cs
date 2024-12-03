using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using UnityEngine.Rendering.Universal;
using Newtonsoft.Json;
using System.IO;



public class StageState : IGameState
{
    public bool IsStarted => _isStarted;
    private bool _isStarted = false;

    public int currentStage;
    public int currentIndex;

    // ��� �ʱ�ȭ ��
    const int INIT_LIFE_COUNT = 9;
    private int _lifeCount = INIT_LIFE_COUNT;
    private float _gridYLowerBound = -2.0f;

    private TextMeshProUGUI _timerText;
    private float _timeLimit = 300f;
    private float _remainingTime;
    private float _accumulativeTime = 0f;

    private GameObject _player;
    private CinemachineVirtualCamera _virtualCamera;
    private int gridSizeX;
    private int gridSizeY;
    Dictionary<(int stage, int index), (int gridSizeX, int gridSizeY)> stageGridSizes =
        new Dictionary<(int, int), (int, int)>
        {
        };

    public int totalCatnipCount;
    public int collectedCatnipCount;
    public List<bool> isCatnipCollected = new List<bool>();
    private List<GameObject> _catnipIcons = new List<GameObject>();
    private GameObject catnipIconPrefab;
    private Transform catnipIconContainer;

    private int enteredPipeID = -1;

    public bool respawnPositionIsStartPoint = false;
    public Vector3 respawnPosition;
    public bool isRespawnPositionSetted = false;
    private GameObject playerPrefab;

    //���� ���� ��ü
    UserData _userData;

    public StageState(int stage)
    {
        currentStage = stage;
        currentIndex = 1;
        playerPrefab = Resources.Load<GameObject>("PlayerController");
        catnipIconPrefab = Resources.Load<GameObject>("CatnipIcon");
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerController prefab�� ã�� �� �����ϴ�.");
        }

        //load all grid sizes on stage construction
        LoadGridSize(currentStage);

        //���� ���� ��ü �ʱ�ȭ �� ��Ȳ �ε�
        _userData = new UserData();
        _userData.LoadGameData();

        //��� �ҷ����� �� �ֱ� �������� ����
        _lifeCount = _userData.lives;
        _userData.UpdateRecentStage(currentStage);
    }

    public void Start()
    {
        _isStarted = true;
        _remainingTime = _timeLimit;

        //���� ���� ��ü �ʱ�ȭ �� ��Ȳ �ε�
        _userData = new UserData();
        _userData.LoadGameData();

        //��� �ҷ����� �� �ֱ� �������� ����
        _lifeCount = _userData.lives;
        _userData.UpdateRecentStage(currentStage);

        InitializeCatnipInfo();
        FindCatnipIconContainer();
        PlaceCatnipIcons();
        SpawnPlayerAtStartPoint();
        UpdateStageMapSize();
    }

    public void Update()
    {
        if (!_isStarted)
            return;

        _remainingTime -= Time.deltaTime;
        UpdateTimerUI(_remainingTime);

        if (_player == null || _virtualCamera == null)
        {
            _player = GameObject.FindWithTag("Player");
            _virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            UpdateCameraTargetToPlayer();
        }

        if (_remainingTime <= 0f)
        {
            KillPlayer();
            return;
        }

        if (_player != null)
        {
            Vector3 playerPosition = _player.transform.position;
            if (playerPosition.y < _gridYLowerBound)
            {
                KillPlayer();
                return;
            }
        }
    }

    public void Exit(ExitState exitState)
    {
        switch (exitState)
        {
            case ExitState.StageClear:
                StageClear();
                break;

            case ExitState.GameOver:
                // GameOver ���� �� �͵� ���߿� �߰�
                // ��� ���� �� ����
                _lifeCount = INIT_LIFE_COUNT;
                _userData.UpdateLives(_lifeCount);
                break;

            default:
                break;
        }
        _isStarted = false;
    }

    private void StageClear()
    {
        GameManager.Instance.StartNewStageWithEvent(currentStage + 1);

        //�������� ��� ����
        _accumulativeTime += _timeLimit - _remainingTime;
        _userData.SaveStageData(currentStage, Mathf.FloorToInt(_accumulativeTime), isCatnipCollected);
    }

    private void UpdateCameraTargetToPlayer()
    {
        if (_player != null && _virtualCamera != null)
        {
            _virtualCamera.Follow = _player.transform;
            _virtualCamera.OnTargetObjectWarped(_player.transform, _player.transform.position - _virtualCamera.transform.position);
        }
        else
        {
            Debug.LogError("Player �Ǵ� Camera�� ã�� �� ����");
        }
    }

    // load all grid sizes on stage construction
    private void LoadGridSize(int stage) 
    {
        for (int index = 1; ; index++)
        {
            string fileName = $"Stage_{stage}_{index}";
            string path = Path.Combine("TerrainData", fileName);
            var file = Resources.Load<TextAsset>(path);

            if (file == null)
                break;

            string json = file.text;
            TerrainData terrainData = JsonConvert.DeserializeObject<TerrainData>(json);

            stageGridSizes[(stage, index)] = (terrainData.gridSize.x, terrainData.gridSize.y);
        }
    }
    private void UpdateStageMapSize()
    {
        if (stageGridSizes.TryGetValue((currentStage, currentIndex), out var gridSize))
        {
            gridSizeX = gridSize.gridSizeX;
            gridSizeY = gridSize.gridSizeY;
        }
        _virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        var confiner = _virtualCamera.GetComponent<CinemachineConfiner>();
        if (confiner != null)
        {
            var colliderObject = new GameObject("Confiner");
            colliderObject.transform.position = Vector3.zero;
            colliderObject.layer= LayerMask.NameToLayer("Invincible");

            // Add a PolygonCollider2D to define the bounding shape
            var boxCollider = colliderObject.AddComponent<BoxCollider>();

            // Calculate the confiner bounds based on the virtual camera's orthographic size
            var gridOffsetX = _virtualCamera.m_Lens.OrthographicSize * Screen.width / Screen.height;
            var gridOffsetY = _virtualCamera.m_Lens.OrthographicSize;

            boxCollider.size = new Vector3(gridSizeX, gridSizeY, 100);
            boxCollider.center = new Vector3(gridSizeX / 2-0.5f, gridSizeY / 2-0.5f, 0);

            confiner.m_BoundingVolume = boxCollider;

            Debug.Log("Confiner setup complete.");
        }
    }

    /******************** �÷��̾� ���� ���� �Լ��� ********************/

    private void LifeOver()
    {
        _lifeCount--;
        _userData.UpdateLives(_lifeCount);

        //game over
        if (_lifeCount <= 0)
        {
            ClearCatnipUI();
            GameManager.Instance.GameOverWithEvent();
            return;
        }

        SceneManager.sceneLoaded += OnCurrentSceneLoaded;
        SceneLoader.LoadCurrentScene();
    }

    private void OnCurrentSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �ʱ�ȭ �۾�
        FindTimerText();
        FindCatnipIconContainer();
        PlaceCatnipIcons();

        if (respawnPositionIsStartPoint)
        {
            SpawnPlayerAtStartPoint();
        }
        else
        {
            SpawnPlayer(respawnPosition);
        }
        UpdateStageMapSize();
        SceneManager.sceneLoaded -= OnCurrentSceneLoaded;
    }

    private void KillPlayer()
    {
        if (_remainingTime <= 0f)
        {
            _accumulativeTime += _timeLimit - _remainingTime;
            _remainingTime = _timeLimit;
        }
        LifeOver();
    }

    /******************** �÷��̾� Spawn ���� �Լ��� ********************/

    public void SpawnPlayer(Vector3 position)
    {
        Debug.LogWarning($"Spawning player at {position}");
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.Log($"Player spawned : {position}");
            player = PoolManager.Instance.Pool(playerPrefab, position, Quaternion.identity);
        }
        else
        {
            player.transform.position = position;
            Debug.Log($"Player moved to position: {position}");
        }

        //�⺻ �̺�Ʈ ����
        player.GetComponent<BattleModule>().death.AddListener(LifeOver);
    }

    private void SpawnPlayerAtStartPoint()
    {
        GameObject startPointObject = GameObject.FindWithTag("StartPoint");
        StartPoint startPoint = startPointObject.GetComponent<StartPoint>();
        startPoint?.Initialize();
    }

    public void UpdateRespawnPosition(Vector3 position, bool respawnPositionIsStartPoint)
    {
        respawnPosition = position;
        this.respawnPositionIsStartPoint = respawnPositionIsStartPoint;
        Debug.Log($"respawnposition set to {respawnPosition}");
    }

    /******************** Pipe ���� �Լ��� ********************/

    public void GoTargetIndexByPipe(int index, int targetPipeID)
    {
        currentIndex = index;
        enteredPipeID = targetPipeID;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneLoader.LoadTargetStage(currentStage, currentIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindTimerText();
        FindCatnipIconContainer();
        PlaceCatnipIcons();

        Pipe targetPipe = PipeUtils.FindPipeByID(enteredPipeID);
        enteredPipeID = -1;
        if (targetPipe != null)
        {
            targetPipe.GetComponent<Pipe>().Initialize();
        }
        else
        {
            Debug.LogError("���� ������ target Pipe�� ã�� �� �����ϴ�.");
        }
        UpdateStageMapSize();
        Debug.LogError($"gridSize: {gridSizeX}, {gridSizeY}");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /******************** Ÿ�̸� ���� �Լ��� ********************/

    public void FindTimerText()
    {
        GameObject timerObject = GameObject.FindGameObjectWithTag("TimerText");
        if (timerObject != null)
        {
            _timerText = timerObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("TimerText UI�� ã�� �� �����ϴ�.");
        }
    }

    public void UpdateTimerUI(float remainingTime)
    {
        if (_timerText != null)
        {
            _timerText.text = $"{Mathf.CeilToInt(remainingTime)}";
        }
        else
        {
            FindTimerText();
        }
    }

    /******************** Catnip ���� �Լ��� ********************/

    private void InitializeCatnipInfo()
    {
        totalCatnipCount = CatnipUtils.CountTotalCatnipInStage(currentStage);
        Debug.Log($"�� Ĺ�� ���� : {totalCatnipCount}");
        InitializeCatnipCollectionStates(totalCatnipCount);
        collectedCatnipCount = 0;
    }

    private void InitializeCatnipCollectionStates(int count)
    {
        isCatnipCollected = new List<bool>(new bool[count]);
    }

    private void FindCatnipIconContainer()
    {
        if (catnipIconContainer == null)
        {
            GameObject containerObject = GameObject.FindGameObjectWithTag("CatnipIconContainer");
            if (containerObject != null)
            {
                catnipIconContainer = containerObject.transform;
            }
            else
            {
                Debug.LogWarning("CatnipIconContainer�� ã�� �� �����ϴ�.");
            }
        }
    }

    private void PlaceCatnipIcons()
    {
        ClearCatnipUI();
        Debug.LogWarning($"_catnipIcon ����Ʈ ���� ����: {_catnipIcons.Count}");
        for (int i = 0; i < totalCatnipCount; i++)
        {
            GameObject icon = PoolManager.Instance.Pool(catnipIconPrefab, Vector3.zero, Quaternion.identity, catnipIconContainer);
            icon.transform.SetParent(catnipIconContainer, false);
            _catnipIcons.Add(icon);
            SetCatnipIconState(icon, isCatnipCollected[i]);
        }
    }

    public void CollectCatnipInStageState(int catnipID)
    {
        collectedCatnipCount++;
        UpdateCatnipStateToCollected(catnipID);
    }

    public void UpdateCatnipStateToCollected(int catnipID)
    {
        if (catnipID > 0 && catnipID <= _catnipIcons.Count)
        {
            isCatnipCollected[catnipID - 1] = true;
            SetCatnipIconState(_catnipIcons[catnipID - 1], true);
        }
        else
        {
            Debug.LogWarning("�������� catnipID: " + catnipID);
        }
    }

    public void ClearCatnipUI()
    {
        _catnipIcons.Clear();
    }

    private void SetCatnipIconState(GameObject icon, bool isActive)
    {
        if (icon == null)
        {
            Debug.LogError("�ش� catnip Icon�� ã�� �� ����");
            return;
        }
        Color iconColor = icon.GetComponent<UnityEngine.UI.Image>().color;
        iconColor.a = isActive ? 1.0f : 0.5f;
        icon.GetComponent<UnityEngine.UI.Image>().color = iconColor;
    }
}
