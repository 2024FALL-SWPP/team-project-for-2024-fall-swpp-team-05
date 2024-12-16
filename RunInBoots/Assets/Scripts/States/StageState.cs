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
    const int InitLifeCount = 9;
    private int _lifeCount = InitLifeCount;
    private const float GridLowerYBound = -2.0f;

    private TextMeshProUGUI _timerText;
    private const float TimeLimit = 300f;
    private float _remainingTime;
    private float _accumulativeTime = 0f;

    private GameObject _player;
    private StageCamera _stageCamera;

    private CatnipModule catnipModule;
    
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
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerController prefab�� ã�� �� �����ϴ�.");
        }

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
        _remainingTime = TimeLimit;

        //���� ���� ��ü �ʱ�ȭ �� ��Ȳ �ε�
        _userData = new UserData();
        _userData.LoadGameData();

        //��� �ҷ����� �� �ֱ� �������� ����
        _lifeCount = _userData.lives;
        _userData.UpdateRecentStage(currentStage);

        catnipModule = new CatnipModule(currentStage);
        
        StageUIUtils.PlaceHeartIcons(_lifeCount);
        SpawnPlayerAtStartPoint();

        //find stage camera
        FindStageCamera();
        _stageCamera.UpdateStageMapSize(currentStage, currentIndex);

        FindTimerText();
    }

    public void Update()
    {
        if (!_isStarted)
            return;

        _remainingTime -= Time.deltaTime;
        UpdateTimerText();

        if (_remainingTime <= 0f)
        {
            _accumulativeTime += TimeLimit - _remainingTime;
            _remainingTime = TimeLimit;
            LifeOverWithEvent();
            return;
        }

        if (_player != null)
        {
            Vector3 playerPosition = _player.transform.position;
            if (playerPosition.y < GridLowerYBound)
            {
                Debug.Log("Player fell off the map.");
                LifeOverWithEvent();
                return;
            }
        }
    }

    public void Exit(eExitState exitState)
    {
        switch (exitState)
        {
            case eExitState.StageClear:
                StageClear();
                break;

            case eExitState.GameOver:
                // GameOver ���� �� �͵� ���߿� �߰�
                // ��� ���� �� ����
                _lifeCount = InitLifeCount;
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
        _accumulativeTime += TimeLimit - _remainingTime;
        _userData.SaveStageData(currentStage, Mathf.FloorToInt(_accumulativeTime), (List<bool>)GetIsCatnipCollected());
    }

    /******************** �÷��̾� ���� ���� �Լ��� ********************/

    private void FindStageCamera() 
    {
        if (_stageCamera != null) return;
        _stageCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>().GetComponent<StageCamera>();
        _stageCamera.LoadGridSize(currentStage);
    }

    private void LifeOver()
    {
        _lifeCount--;
        _userData.UpdateLives(_lifeCount);

        //unequip hat if exists
        currentHatType = eHatType.None;

        //game over
        if (_lifeCount <= 0)
        {
            catnipModule.ClearCatnipIcons();
            GameManager.Instance.GameOver();
            return;
        }

        SceneManager.sceneLoaded += OnCurrentSceneLoaded;
        SceneLoader.LoadCurrentScene();
    }

    private void LifeOverWithEvent()
    {
        GameObject player = _player;
        _player = null;
        ProducingEvent gameOverEvent = EventUtils.DeathEvent();
        gameOverEvent.AddEndEvent(() =>
        {
            player.GetComponent<Rigidbody>().useGravity = true;
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1.0f);
            ProducingEvent delayEvent = new DelayEvent(1.0f);
            delayEvent.AddEndEvent(() =>
            {
                ProducingEvent blackScreenEvent = EventUtils.BlackScreenEvent();
                blackScreenEvent.AddEndEvent(() =>
                {
                    Debug.Log("LifeOver Event End");
                    if (player != null) player.SetActive(false);
                    LifeOver();
                });
                GameManager.Instance.AddEvent(blackScreenEvent);
            });
            GameManager.Instance.AddEvent(delayEvent);
        });
        GameManager.Instance.AddEvent(gameOverEvent);
    }

    private void OnCurrentSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �ʱ�ȭ �۾�
        FindTimerText();
        catnipModule.PlaceCatnipIcons();
        StageUIUtils.PlaceHeartIcons(_lifeCount);

        if (respawnPositionIsStartPoint)
        {
            SpawnPlayerAtStartPoint();
        }
        else
        {
            SpawnPlayerWithEvent(respawnPosition);
        }
        FindStageCamera();
        _stageCamera.UpdateStageMapSize(currentStage, currentIndex);
        SceneManager.sceneLoaded -= OnCurrentSceneLoaded;
    }

    /******************** �÷��̾� Spawn ���� �Լ��� ********************/

    public void SpawnPlayer(Vector3 position)
    {
        Debug.LogWarning($"Spawning player at {position}");
        DisablePipeColliderTemporarilyIfExists(position);

        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
            Debug.Log($"Player spawned : {position}");
            // raycast ground beneath the spawn position
            _player = PoolManager.Instance.Pool(playerPrefab, position + 0.5f*Vector3.up, Quaternion.identity);
            //�⺻ �̺�Ʈ ����
            _player.GetComponent<BattleModule>().death.AddListener(LifeOverWithEvent);
            _player.GetComponent<CamouflageModule>().InitializeBattleModule();
            _player.GetComponent<CamouflageModule>().onChangeHat.AddListener(() => {
                currentHatType = _player.GetComponent<CamouflageModule>().GetCurrentHatType();
            });
            _player.GetComponent<CamouflageModule>().Initialize(currentHatType);
            FindStageCamera();
            _stageCamera.UpdateCameraTargetToPlayer(_player);
        }
        else
        {
            _player.transform.position = position - 0.5f*Vector3.up;
            Debug.Log($"Player moved to position: {position}");
        }
    }

    public void SpawnPlayerWithEvent(Vector3 spawnPosition)
    {
        ProducingEvent spawnEvent = EventUtils.SpawnEvent(currentStage, _lifeCount, spawnPosition);
        spawnEvent.InsertStartEvent(() =>
        {
            SpawnPlayer(spawnPosition);
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
        currentHatType = _player.GetComponent<CamouflageModule>().GetCurrentHatType();
        Debug.Log($"{_player.name}이(가) {currentHatType} 상태로 {index}번째 파이프로 이동합니다.");
        currentIndex = index;
        enteredPipeID = targetPipeID;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneLoader.LoadTargetStage(currentStage, currentIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindTimerText();
        catnipModule.PlaceCatnipIcons();
        StageUIUtils.PlaceHeartIcons(_lifeCount);

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
        FindStageCamera();
        _stageCamera.UpdateStageMapSize(currentStage,currentIndex);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /******************** Ÿ�̸� ���� �Լ��� ********************/

    private void FindTimerText() 
    { 
        _timerText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<TextMeshProUGUI>();
        UpdateTimerText();
    }
    private void UpdateTimerText()
    {
        if (_timerText != null) _timerText.text = $"{Mathf.CeilToInt(_remainingTime)}";
    }

    /******************** Catnip ���� �Լ��� ********************/

    public void CollectCatnipInStageState(int catnipID)
    {
        catnipModule.UpdateCatnipStateToCollected(catnipID);
    }

    public IReadOnlyList<bool> GetIsCatnipCollected() => catnipModule.GetIsCatnipCollected();
}
