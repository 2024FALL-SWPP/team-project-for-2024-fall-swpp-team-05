using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using UnityEngine.Rendering.Universal;



public class StageState : IGameState
{
    public bool IsStarted => _isStarted;
    private bool _isStarted = false;

    public int currentStage;
    public int currentIndex;

    private int _lifeCount = 9;
    private float _gridYLowerBound = -2.0f;

    private TextMeshProUGUI _timerText;
    private float _timeLimit = 300f;
    private float _remainingTime;
    private float _accumulativeTime = 0f;

    private GameObject _player;
    private CinemachineVirtualCamera _virtualCamera;

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

    public StageState(int stage)
    {
        currentStage = stage;
        currentIndex = 1;
        playerPrefab = Resources.Load<GameObject>("PlayerController");
        catnipIconPrefab = Resources.Load<GameObject>("CatnipIcon");
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerController prefab을 찾을 수 없습니다.");
        }
    }

    public void Start()
    {
        _isStarted = true;
        _remainingTime = _timeLimit;

        InitializeCatnipInfo();
        FindCatnipIconContainer();
        PlaceCatnipIcons();
        SpawnPlayerAtStartPoint();
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
            UpdateCameraTarget();
        }

        if (_remainingTime <= 0f)
        {
            KillPlayer();
            return;
        }

        if (_player != null)
        {
            if (_player.transform.position.y < _gridYLowerBound)
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
                // GameOver 관련 할 것들 나중에 추가
                break;

            default:
                break;
        }
        _isStarted = false;
    }

    private void StageClear()
    {
        GameManager.Instance.StartNewStage(currentStage + 1);
    }

    private void UpdateCameraTarget()
    {
        if (_player != null && _virtualCamera != null)
        {
            _virtualCamera.Follow = _player.transform;
            _virtualCamera.OnTargetObjectWarped(_player.transform, _player.transform.position - _virtualCamera.transform.position);
        }
        else
        {
            Debug.LogError("Player 또는 Camera를 찾을 수 없음");
        }
    }

    /******************** 플레이어 죽음 관련 함수들 ********************/

    private void LifeOver()
    {
        _lifeCount--;
        SceneManager.sceneLoaded += OnCurrentSceneLoaded;
        SceneLoader.LoadCurrentScene();
    }

    private void OnCurrentSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 초기화 작업
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
        SceneManager.sceneLoaded -= OnCurrentSceneLoaded;
    }

    private void KillPlayer()
    {
        if (_remainingTime <= 0f)
        {
            _accumulativeTime += _timeLimit - _remainingTime;
            _remainingTime = _timeLimit;
        }

        if (_lifeCount > 1)
        {
            LifeOver();
        }
        else
        {
            _lifeCount = 9;
            ClearCatnipUI();

            GameManager.Instance.GameOver();
        }
    }

    /******************** 플레이어 Spawn 관련 함수들 ********************/

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

    /******************** Pipe 관련 함수들 ********************/

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
            Debug.LogError("다음 씬에서 target Pipe를 찾을 수 없습니다.");
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /******************** 타이머 관련 함수들 ********************/

    public void FindTimerText()
    {
        GameObject timerObject = GameObject.FindGameObjectWithTag("TimerText");
        if (timerObject != null)
        {
            _timerText = timerObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("TimerText UI를 찾을 수 없습니다.");
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

    /******************** Catnip 관련 함수들 ********************/

    private void InitializeCatnipInfo()
    {
        totalCatnipCount = CatnipUtils.CountTotalCatnipInStage(currentStage);
        Debug.Log($"총 캣닢 개수 : {totalCatnipCount}");
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
                Debug.LogWarning("CatnipIconContainer를 찾을 수 없습니다.");
            }
        }
    }

    private void PlaceCatnipIcons()
    {
        ClearCatnipUI();
        Debug.LogWarning($"_catnipIcon 리스트 원소 개수: {_catnipIcons.Count}");
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
            Debug.LogWarning("부적절한 catnipID: " + catnipID);
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
            Debug.LogError("해당 catnip Icon을 찾을 수 없음");
            return;
        }
        Color iconColor = icon.GetComponent<UnityEngine.UI.Image>().color;
        iconColor.a = isActive ? 1.0f : 0.5f;
        icon.GetComponent<UnityEngine.UI.Image>().color = iconColor;
    }
}
