using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private IGameState _currentState;

    public int enteredPipeID;
    public bool isComingFromPipe;
    // Start is called before the first frame update
    void Start()
    {
        _currentState = new StageState();
        _currentState.Start();
    }

    // Update is called once per frame  
    void Update()
    {
        if (_currentState != null && _currentState.IsStarted)
        {
            _currentState.Update();
        }
    }

    public void GameOver()
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }
    }
}
