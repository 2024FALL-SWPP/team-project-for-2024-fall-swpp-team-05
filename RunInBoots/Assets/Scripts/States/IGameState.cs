using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExitState
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
    void Exit(ExitState exitState);
}