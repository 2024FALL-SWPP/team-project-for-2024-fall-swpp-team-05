using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoSingleton<GameManager>
{
    public IGameState currentState;

    private Queue<ProducingEvent> _eventQueue = new Queue<ProducingEvent>();
    private ProducingEvent _currentEvent;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        TestCamouflageModule.Test();
    }

    public void StartNewStage(int stage)
    {
        if (!SceneLoader.LoadTargetStage(stage, 1))
        {
            AllStageClear();
            return;
        }
        currentState = new StageState(stage);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        currentState.Start();
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
        if (currentState != null && currentState.IsStarted && _currentEvent == null)
        {
            _eventQueue.Clear();
            currentState.Update();
        }
    }

    // 모든 스테이지 클리어 된 후 동작 나중에 채워넣기
    private void AllStageClear() 
    {
        currentState = null;
        SceneLoader.LoadTitleScene();
        Debug.Log("Game Clear");
    }

    public void GameOver()
    {
        if (currentState != null)
        {
            currentState.Exit(ExitState.GameOver);
        }
        SceneLoader.LoadTitleScene();
    }

    public StageState GetCurrentStageState()
    {
        var stageState = currentState as StageState;
        if (stageState == null)
        {
            Debug.LogWarning("currentState is null or not of type StageState.");
        }
        return currentState as StageState;
    }

    // Event Queue
    public void AddEvent(ProducingEvent producingEvent)
    {
        _eventQueue.Enqueue(producingEvent);
    }

    public void GetObject(GameObject obj, Vector3 offset)
    {
        // Move object position to player position (+ offset)
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && obj != null)
        {
            obj.transform.position = player.transform.position + offset;
        }
    }
}
