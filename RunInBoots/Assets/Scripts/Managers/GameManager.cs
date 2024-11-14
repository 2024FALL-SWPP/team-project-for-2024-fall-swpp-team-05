using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private IGameState _currentState;

    public int enteredPipeID = -1;
    public bool isComingFromPipe = false;
    private GameObject playerPrefab;

    protected override void Awake()
    {
        base.Awake();

        playerPrefab = Resources.Load<GameObject>("PlayerController");
        if(playerPrefab == null)
        {
            Debug.LogError("PlayerController prefab이 Resources/에 존재하지 않음");
        }
    }

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

    public void SpawnPlayer(Vector3 position)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.Log("GameManager.SpawnManager에서 Player를 못 찾음");
            player = Instantiate(playerPrefab, position, Quaternion.identity);
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
