using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    private IGameState _currentState;

    private GameObject playerPrefab;

    public int enteredPipeID = -1;
    public bool isComingFromPipe = false;

    public int totalCatnipCount;
    public int collectedCatnipCount;
    public List<bool> isCatnipCollected = new List<bool>();
    
    private int _lifeCount = 9;
    public Vector3 respawnPosition;
    public bool isRespawnPositionSetted = false;

    private Queue<ProducingEvent> _eventQueue = new Queue<ProducingEvent>();
    private ProducingEvent _currentEvent;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        playerPrefab = Resources.Load<GameObject>("PlayerController");
        if(playerPrefab == null)
        {
            Debug.LogError("PlayerController prefab�� Resources/�� �������� ����");
        }
    }

    public void StartNewStage()
    {
        _currentState = new StageState();
        _currentState.Start();

        int currentStage = GetCurrentStage();
        totalCatnipCount = GameUtils.CountTotalCatnipInStage(currentStage);

        if (isCatnipCollected.Count == 0 || isCatnipCollected.Count != totalCatnipCount)
        {
            InitializeCatnipStates(totalCatnipCount);
        }

        collectedCatnipCount = 0;
        UIManager.Instance.InitializeCatnipUI(totalCatnipCount);
    }

    // Update is called once per frame  
    void Update()
    {
        if(_currentEvent == null && _eventQueue.Count > 0)
        {
            _currentEvent = _eventQueue.Dequeue();
            _currentEvent.Start();
        }
        else if(_currentEvent != null && _currentEvent.isEnded == false)
        {
            _currentEvent.Update();
        }
        else if(_currentEvent != null && _currentEvent.isEnded)
        {
            _currentEvent.Exit();
            if(_eventQueue.Count > 0)
            {
                _currentEvent = _eventQueue.Dequeue();
                _currentEvent.Start();
            }
            else
            {
                _currentEvent = null;
            }
        }

        if (_currentState != null && _currentState.IsStarted && _currentEvent == null)
        {
            _eventQueue.Clear();
            _currentState.Update();
        }
    }

    

    public void StageClear()
    {
        UIManager.Instance.ClearCatnipUI();
        if (_currentState != null)
        {
            _currentState.Exit();
            isRespawnPositionSetted = false;
            if (!LoadNextStage(GetCurrentStage()))
            {
                GameClear();
            }
        }
    }
    private bool LoadNextStage(int currentStage)
    {
        // ���� ���������� �����ϴ� ���� ���� �ִ��� Ȯ��
        string nextSceneName = $"Stage_{currentStage + 1}_{1}";

        // �ش� ���� �ε� ������ �������� Ȯ��
        if (SceneUtility.GetBuildIndexByScenePath(nextSceneName) != -1)
        {
            SceneManager.LoadScene(nextSceneName);
            Debug.Log($"Loading next stage: {nextSceneName}");
            return true;
        }
        else
        {
            Debug.Log("No next stage available. Ending current stage.");
            return false;
        }
    }

    private void GameClear() 
    {
        Debug.Log("Game Clear");
    }
    private void LifeOver()
    {
        _lifeCount--;

        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);

        SpawnPlayer(respawnPosition);
    }

    private void GameOver()
    {
        UIManager.Instance.ClearCatnipUI();
        _lifeCount = 9;
        SceneManager.LoadScene("TitleScene");
        InitializeCatnipStates(totalCatnipCount);
        if (_currentState != null)
        {
            _currentState.Exit();
        }
    }

    public void CollectCatnip(int catnipID)
    {
        collectedCatnipCount++;
        UIManager.Instance.UpdateCatnipUI(catnipID);
        
    }

    public void InitializeCatnipStates(int count)
    {
        isCatnipCollected = new List<bool>(new bool[count]);
    }

    public void UpdateCatnipState(int catnipID)
    {
        if (catnipID > 0 && catnipID <= isCatnipCollected.Count)
        {
            isCatnipCollected[catnipID - 1] = true;
        }
    }



    public int GetCurrentStage()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string[] parts = sceneName.Split('_');
        if (parts.Length > 1 && int.TryParse(parts[1], out int stage))
        {
            return stage;
        }
        return -1;
    }

    public int GetCurrentIndex()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string[] parts = sceneName.Split('_');
        if (parts.Length > 1 && int.TryParse(parts[2], out int index))
        {
            return index;
        }
        return -1;
    }

    public void SpawnPlayer(Vector3 position)
    {
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

    public void ResetPipeData()
    {
        enteredPipeID = -1;
        isComingFromPipe = false;
    }

    public void HandlePlayerDeath(bool isTimeout)
    {
        if (_lifeCount > 1)
        {
            LifeOver();
        }
        else
        {
            GameOver();
        }
    }

    
}
