using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트는 본 수업 이전 개인이 진행했던 프로젝트에서 가져왔음을 알림.

public interface IGameState
{
    public bool IsStarted { get; }
    void Start();
    void Update();
    void Exit();
}