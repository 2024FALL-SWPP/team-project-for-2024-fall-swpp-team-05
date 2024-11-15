using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using TMPro;    

public class StageState : IGameState
{
    public bool IsStarted => _isStarted;
    private bool _isStarted = false;

    private float _timeLimit = 300f;
    private float _remainingTime;

    private GameObject _player;
    private CinemachineVirtualCamera _virtualCamera;

    private float _gridYLowerBound = -2.0f;

    private GameObject _levelObject;

    

    public void Start()
    {
        _isStarted = true;
        _remainingTime = _timeLimit;
        _levelObject = GameObject.Find("LevelObject");
        if(_levelObject != null)
        {
            _levelObject.SetActive(true);
            Debug.Log("Level Object load");
        }
        else
        {
            Debug.Log("Level Object loading failed");
        }

        _virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>(); if (_virtualCamera != null)
        {
            Debug.Log("Find virtual camera");
            _player = GameObject.FindWithTag("Player");


            if (_player != null)
            {
                _virtualCamera.Follow = _player.transform;
                _virtualCamera.OnTargetObjectWarped(_player.transform, _player.transform.position - _virtualCamera.transform.position);
            }
            else
            {
                Debug.LogWarning("Player를 찾을 수 없습니다.");
            }
            
            UIManager.Instance.UpdateTimerUI(_remainingTime);
        }
        else
        {
            Debug.LogWarning("PlayerCamera를 찾을 수 없습니다.");
        }
    }

    public void Update()
    {
        if (!_isStarted)
            return;

        _remainingTime -= Time.deltaTime;
        UIManager.Instance.UpdateTimerUI(_remainingTime);

        if (_remainingTime <= 0f)
        {
            Exit();
            return;
        }

        if (_player != null)
        {
            if (_player.transform.position.y < _gridYLowerBound)
            {
                Exit();
                return;
            }
        }
    }
    public void Exit()
    {
        _isStarted = false;
        // (임시) 현재 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
