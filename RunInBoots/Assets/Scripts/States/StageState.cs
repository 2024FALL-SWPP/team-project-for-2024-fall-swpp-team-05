    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using Cinemachine;
    using TMPro;



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
        private GameObject _levelObject;

        public int totalCatnipCount;
        public int collectedCatnipCount;
        public List<bool> isCatnipCollected = new List<bool>();
        private List<GameObject> _catnipIcons = new List<GameObject>();
        private GameObject catnipIconPrefab;
        private Transform catnipIconContainer;

        public int enteredPipeID = -1;
        public bool isComingFromPipe = false;

        public Vector3 respawnPosition;
        public bool isRespawnPositionSetted = false;
        private GameObject playerPrefab;

        public StageState(int stage)
        {
            currentStage = stage;
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
            _levelObject = GameObject.Find("LevelObject");
            if (_levelObject != null)
            {
                _levelObject.SetActive(true);
                Debug.Log("Level Object load");
            }

            InitializeCatnip();
            FindCatnipIconContainer();
            PlaceCatnipIcons();

            GameObject startPoint = GameObject.FindWithTag("StartPoint");
            startPoint.GetComponent<StartPoint>().Initialize();
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
                    isRespawnPositionSetted = false;
                    ClearCatnipUI();
                    break;

                case ExitState.GameOver:
                    // GameOver 관련 할 것들 나중에 추가
                    break;

                default:
                    break;
            }
            _isStarted = false;
        }

        private void LifeOver()
        {
            _lifeCount--;
            SceneLoader.LoadCurrentScene();
            SpawnPlayer(respawnPosition);
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

        public void UpdateRespawnPosition(Vector3 position)
        {
            respawnPosition = position;
            isRespawnPositionSetted = true;
            Debug.Log($"respawnposition set to {respawnPosition}");
        }

        public void GoTargetIndexByPipe(int index)
        {
            currentIndex = index;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneLoader.LoadTargetStage(currentStage, currentIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        Pipe targetPipe = PipeUtils.FindPipeByID(enteredPipeID);
        if (targetPipe != null)
        {
            targetPipe.GetComponent<Pipe>().Initialize();
        }
        else
        {
            Debug.LogError("다음 씬에서 target Pipe를 찾을 수 없습니다.");
        }

        FindTimerText();
        FindCatnipIconContainer();
        PlaceCatnipIcons();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ResetPipeData()
    {
        enteredPipeID = -1;
        isComingFromPipe = false;
    }

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

    private void InitializeCatnip()
    {
        totalCatnipCount = CatnipUtils.CountTotalCatnipInStage(currentStage);
        //if (isCatnipCollected.Count == 0 || isCatnipCollected.Count != totalCatnipCount)
        //{
        //    CatnipUtils.InitializeCatnipCollectionStates(isCatnipCollected, totalCatnipCount);
        //}
        Debug.LogWarning($"총 캣닢 개수 : {totalCatnipCount}");
        CatnipUtils.InitializeCatnipCollectionStates(isCatnipCollected, totalCatnipCount);
        collectedCatnipCount = 0;
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
        //ClearCatnipUI();

        for (int i = 0; i < totalCatnipCount; i++)
        {
            GameObject icon = PoolManager.Instance.Pool(catnipIconPrefab, Vector3.zero, Quaternion.identity, catnipIconContainer);
            //icon.transform.localPosition = Vector3.zero;
            _catnipIcons.Add(icon);
            SetCatnipIconState(icon, isCatnipCollected[i]);
        }
    }

    public void CollectCatnipInStageState(int catnipID)
    {
        collectedCatnipCount++;
        CatnipUtils.CatnipCollectUIUpdate(catnipID);
        CatnipUtils.UpdateCatnipToCollected(isCatnipCollected, catnipID);
    }

    public void UpdateCatnipUI(int catnipID)
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
        foreach (GameObject icon in _catnipIcons)
        {
            icon.SetActive(false);
        }
        _catnipIcons.Clear();
    }

    private void SetCatnipIconState(GameObject icon, bool isActive)
    {
        Color iconColor = icon.GetComponent<UnityEngine.UI.Image>().color;
        // 투명도 설정
        iconColor.a = isActive ? 1.0f : 0.5f;
        icon.GetComponent<UnityEngine.UI.Image>().color = iconColor;
    }
}
