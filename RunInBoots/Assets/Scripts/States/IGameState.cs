using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eExitState
{
    StageClear,
    GameClear,
    GameOver
}

public interface IGameState
{
    public bool IsStarted { get; }
    void Start();
    void Update();
    void Exit(eExitState exitState);
}