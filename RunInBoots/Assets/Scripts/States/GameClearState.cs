using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearState : IGameState
{
    public bool IsStarted => _isStarted;
    private bool _isStarted = false;

    public void Start()
    {
        SceneLoader.LoadGameClearScene();
        _isStarted = true;
    }

    public void Update()
    {

    }

    public void Exit(ExitState exitState)
    {
        if (exitState == ExitState.GameClear)
        {
            SceneLoader.LoadTitleScene(); // TitleScene ·Îµå
        }
    }
}
