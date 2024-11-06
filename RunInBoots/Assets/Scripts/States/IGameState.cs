using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� ��ũ��Ʈ�� �� ���� ���� ������ �����ߴ� ������Ʈ���� ���������� �˸�.

public interface IGameState
{
    public bool IsStarted { get; }
    void Start();
    void Update();
    void Exit();
}