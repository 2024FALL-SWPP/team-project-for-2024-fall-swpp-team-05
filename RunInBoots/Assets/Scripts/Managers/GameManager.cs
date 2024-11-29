using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoSingleton<GameManager>
{
    public IGameState currentState;


    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        
    }

    private void Start()
    {
        //StartNewStage(100);
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
        if (currentState != null && currentState.IsStarted)
        {
            currentState.Update();
        }
    }

    // 모든 스테이지 클리어 된 후 동작 나중에 채워넣기
    public void AllStageClear() 
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
}
