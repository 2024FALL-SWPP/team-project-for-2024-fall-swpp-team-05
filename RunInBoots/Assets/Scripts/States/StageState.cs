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
using System.Collections;


public class StageState : IGameState
{
    public bool IsStarted => _isStarted;
    private bool _isStarted = false;

    public int currentStage;
    public int currentIndex;
    public eHatType currentHatType;

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
        currentHatType = eHatType.None;
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
                Debug.Log("Player fell off the map.");
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
        GameManager.Instance.StartNewStage(currentStage + 1);

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
            boxCollider.size = new Vector3(gridSizeX-27.5f*16/9, gridSizeY-27.5f, 100);
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

        //unequip hat if exists
        currentHatType = eHatType.None;

        //game over
        if (_lifeCount <= 0)
        {
            ClearCatnipUI();
            GameManager.Instance.GameOver();
            return;
        }

        SceneManager.sceneLoaded += OnCurrentSceneLoaded;
        SceneLoader.LoadCurrentScene();
    }

    private void LifeOverWithEvent()
    {
        Debug.Log("LifeOverWithEvent");
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        Animator playerAnimator = player.GetComponent<AnimatableUI>().animator;
        player.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_PLAYER_DEATH);
        ProducingEvent gameOverEvent = new AnimatorEvent(playerAnimator);

        gameOverEvent.AddStartEvent(() =>
        {
            Debug.Log("GameOver Event Start");
            if(actionSystem != null) actionSystem.ResumeSelf(false);
        });
        gameOverEvent.AddEndEvent(() =>
        {
            GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
            GameObject blackScreen = Resources.Load<GameObject>("BlackScreenUI");
            GameObject blackScreenObj = PoolManager.Instance.Pool(blackScreen, Vector3.zero, Quaternion.identity, canvas.transform);
            blackScreenObj.transform.SetParent(canvas.transform, false);
            blackScreenObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            Animator screenAnimator = blackScreenObj.GetComponent<Animator>();
            blackScreenObj.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_BLACK_START);
            ProducingEvent blackScreenEvent = new AnimatorEvent(screenAnimator);
            blackScreenEvent.AddEndEvent(() =>
            {
                Debug.Log("GameOver Event End");
                if(player != null) player.SetActive(false);
                LifeOver();
            });
            GameManager.Instance.AddEvent(blackScreenEvent);
        });
        GameManager.Instance.AddEvent(gameOverEvent);
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
            SpawnPlayerWithEvent(respawnPosition);
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
        LifeOverWithEvent();
    }

    /******************** �÷��̾� Spawn ���� �Լ��� ********************/

    public void SpawnPlayer(Vector3 position)
    {
        Debug.LogWarning($"Spawning player at {position}");
        GameObject player = GameObject.FindWithTag("Player");

        DisablePipeColliderTemporarilyIfExists(position);

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
        player.GetComponent<BattleModule>().death.AddListener(LifeOverWithEvent);
        player.GetComponent<CamouflageModule>().InitializeBattleModule();
        player.GetComponent<CamouflageModule>().onChangeHat.AddListener(() => { 
                currentHatType = player.GetComponent<CamouflageModule>().GetCurrentHatType(); 
            });
        player.GetComponent<CamouflageModule>().Initialize(currentHatType);
    }

    public void SpawnPlayerWithEvent(Vector3 spawnPosition)
    {
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player?.GetComponent<ActionSystem>();

        GameObject spawnUI = Resources.Load<GameObject>("SpawnUI");
        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        SpawnUI text = PoolManager.Instance.Pool(spawnUI, Vector3.zero, Quaternion.identity, canvas.transform).GetComponent<SpawnUI>();
        text.transform.SetParent(canvas.transform, false);
        text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        text.UpdateStageText(currentStage);
        text.UpdateLifeText(_lifeCount);

        Animator spawnAnimator = text.GetComponent<Animator>();
        ProducingEvent spawnEvent = new AnimatorEvent(spawnAnimator);

        spawnEvent.AddStartEvent(() =>
        {
            SpawnPlayer(spawnPosition);
            if(player == null || actionSystem == null)
            {
                player = GameObject.FindWithTag("Player");
                actionSystem = player.GetComponent<ActionSystem>();
                actionSystem.ResumeSelf(false);
            }
            else actionSystem.ResumeSelf(false);
        });
        spawnEvent.AddEndEvent(() =>
        {
            if(player == null || actionSystem == null)
            {
                player = GameObject.FindWithTag("Player");
                actionSystem = player.GetComponent<ActionSystem>();
                actionSystem.ResumeSelf(true);
            }
            else actionSystem.ResumeSelf(true);
        });
        GameManager.Instance.AddEvent(spawnEvent);
    }
    
    private void DisablePipeColliderTemporarilyIfExists(Vector3 position)
    {
        // 리스폰 위치 근처에 Pipe가 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(position, 1.0f); // 반경 1.0f 탐색
        foreach (var collider in colliders)
        {
            Pipe pipe = collider.GetComponent<Pipe>();
            if (pipe != null)
            {
                Debug.LogWarning($"{pipe.pipeID} pipe 리스폰 떄 비활성화");
                GameManager.Instance.StartGameManagerCoroutine(pipe.DisableCollisionTemporarily()); return;
                return;
            }
        }
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
        GameObject player = GameObject.FindWithTag("Player");
        currentHatType = player.GetComponent<CamouflageModule>().GetCurrentHatType();
        Debug.Log($"{player.name}이(가) {currentHatType} 상태로 {index}번째 파이프로 이동합니다.");
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
