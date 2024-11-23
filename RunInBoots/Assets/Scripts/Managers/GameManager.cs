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
    public List<bool> _catnipCollectedStates = new List<bool>();
    private int lifeCount = 9;



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

        if (_catnipCollectedStates.Count == 0 || _catnipCollectedStates.Count != totalCatnipCount)
        {
            InitializeCatnipStates(totalCatnipCount);
        }

        collectedCatnipCount = 0;
        UIManager.Instance.InitializeCatnipUI(totalCatnipCount);
    }

    // Update is called once per frame  
    void Update()
    {
        if (_currentState != null && _currentState.IsStarted)
        {
            _currentState.Update();
        }
    }

    public void LifeOver()
    {
        // (�ӽ�) ���� ���� �ٽ� �ε�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // ���� ��� ���� ó��, ��� 0�̸� ���� ����, �ƴϸ� ��� UI ���� ���� �� �ٽ� �ε�
    }
    private void GameOver() 
    {
        UIManager.Instance.ClearCatnipUI();
        if (_currentState != null)
        {
            _currentState.Exit();
        }
    }

    public void StageClear()
    {
        UIManager.Instance.ClearCatnipUI();
        if (_currentState != null)
        {
            _currentState.Exit();
            if(!LoadNextStage(GetCurrentStage()))
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

    public void CollectCatnip(int catnipID)
    {
        collectedCatnipCount++;
        UIManager.Instance.UpdateCatnipUI(catnipID);
    }

    public void InitializeCatnipStates(int count)
    {
        _catnipCollectedStates = new List<bool>(new bool[count]);
    }

    public void UpdateCatnipState(int catnipID)
    {
        if (catnipID > 0 && catnipID <= _catnipCollectedStates.Count)
        {
            _catnipCollectedStates[catnipID - 1] = true;
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
            Debug.Log("GameManager.SpawnManager���� Player�� �� ã��");
            player = PoolManager.Instance.Pool(playerPrefab, position, Quaternion.identity);
        }
        else
        {
            player.transform.position = position;
            Debug.Log($"Player moved to position: {position}");
        }
    }

    public void ResetPipeData()
    {
        enteredPipeID = -1;
        isComingFromPipe = false;
    }
}
