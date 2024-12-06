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
        // TestCamouflageModule.Test();
    }

    public void StartNewStageWithEvent(int stage)
    {   
        ProducingEvent stageClearEvent = new AnimatorEvent(null);
        ProducingEvent blackScreenEvent = new AnimatorEvent(null);
        GameObject goalPoint = GameObject.Find("AGoalPoint");
        stageClearEvent.AddStartEvent(() =>
        {
            // Debug.Log("Stage Clear Event Start");
            StopPlayer();
            GetObject(goalPoint, new Vector3(0, 3.0f, 0));
        });
        blackScreenEvent.AddEndEvent(() =>
        {
            // Debug.Log("Stage Clear Event End");
            ResumePlayer();
            StartNewStage(stage);
        });
        AddEvent(stageClearEvent);
        AddEvent(blackScreenEvent);
    }

    public void StartNewStage(int stage)
    {
        if (!SceneLoader.LoadTargetStage(stage, 1))
        {
            // AllStageClear();
            AllStageClearWithEvent();
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

    public void AllStageClearWithEvent()
    {
        ProducingEvent gameClearEvent = new AnimatorEvent(null);
        ProducingEvent blackScreenEvent = new AnimatorEvent(null);
        GameObject goalPoint = GameObject.Find("AGoalPoint");
        gameClearEvent.AddStartEvent(() =>
        {
            // Debug.Log("Game Clear Event Start");
            StopPlayer();
            GetObject(goalPoint, new Vector3(0, 3.0f, 0));
        });
        blackScreenEvent.AddEndEvent(() =>
        {
            // Debug.Log("Game Clear Event End");
            ResumePlayer();
            AllStageClear();
        });
        AddEvent(gameClearEvent);
        AddEvent(blackScreenEvent);
    }

    // 모든 스테이지 클리어 된 후 동작 나중에 채워넣기
    private void AllStageClear() 
    {
        currentState = null;
        SceneLoader.LoadTitleScene();
        Debug.Log("Game Clear");
    }

    public void GameOverWithEvent()
    {
        ProducingEvent gameOverEvent = new AnimatorEvent(null);
        ProducingEvent blackScreenEvent = new AnimatorEvent(null);
        gameOverEvent.AddStartEvent(() =>
        {
            // Debug.Log("GameOver Event Start");
            StopPlayer();
        });
        blackScreenEvent.AddEndEvent(() =>
        {
            // Debug.Log("GameOver Event End");
            GameOver();
        });
        AddEvent(gameOverEvent);
        AddEvent(blackScreenEvent);
    }

    private void GameOver()
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

    public void SpawnPlayerWithEvent(Vector3 spawnPosition)
    {
        ProducingEvent spawnEvent = new AnimatorEvent(null);
        spawnEvent.AddStartEvent(() =>
        {
            // Debug.Log("SpawnPlayerWithEvent Start");
            GetCurrentStageState().SpawnPlayer(spawnPosition);
            StopPlayer();
        });
        spawnEvent.AddEndEvent(() =>
        {
            ResumePlayer();
            // Debug.Log("SpawnPlayerWithEvent End");
        });
        AddEvent(spawnEvent);
    }

    public void CollectCatnipWithEvent(int catnipID)
    {
        ProducingEvent catnipEvent = new AnimatorEvent(null);
        GameObject player = GameObject.FindWithTag("Player");
        GameObject catnip = GameObject.Find($"Catnip_{catnipID}");
        catnipEvent.AddStartEvent(() =>
        {
            // Debug.Log("Catnip Event Start");
            StopPlayer();
            GetObject(catnip, new Vector3(0, 3.0f, 0));
        });
        catnipEvent.AddEndEvent(() =>
        {
            ResumePlayer();
            MakePlayerInvincible();
            GetCurrentStageState().CollectCatnipInStageState(catnipID);
            catnip.SetActive(false);
            // Debug.Log("Catnip Event End");
        });
        AddEvent(catnipEvent);
    }

    // Event Queue
    public void AddEvent(ProducingEvent producingEvent)
    {
        _eventQueue.Enqueue(producingEvent);
    }
    
    public void StopPlayer()
    {
        // Stop player movement and actions
        GameObject player = GameObject.FindWithTag("Player");
        if(player != null)
        {
            // disable player collider
            player.GetComponent<BoxCollider>().enabled = false;
            // disable player movement
            player.GetComponent<TransformModule>().enabled = false;
            // disable player attack
            player.GetComponent<BattleModule>().enabled = false;
            // disable player action
            player.GetComponent<ActionSystem>().enabled = false;
            // set player velocity to zero
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void ResumePlayer()
    {
        // Resume player movement and actions
        GameObject player = GameObject.FindWithTag("Player");
        if(player != null)
        {
            // enable player collider
            player.GetComponent<BoxCollider>().enabled = true;
            // enable player movement
            player.GetComponent<TransformModule>().enabled = true;
            // enable player attack
            player.GetComponent<BattleModule>().enabled = true;
            // enable player action
            player.GetComponent<ActionSystem>().enabled = true;
        }
    }

    public void MakePlayerInvincible()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if(player != null)
        {
            player.GetComponent<BattleModule>().BeInvinvible();
        }
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
