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
    private TextMeshProUGUI  _timerText;

    private float _gridYLowerBound = -10f;

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
            GameObject timerObject = GameObject.Find("TimerText"); // 타이머 UI 텍스트의 이름이 "TimerText"라고 가정
            if (timerObject != null)
            {
                Debug.Log("Find TimerText");
                _timerText = timerObject.GetComponent<TextMeshProUGUI>();
                UpdateTimerUI();
            }
            else
            {
                Debug.LogWarning("TimerText UI 요소를 찾을 수 없습니다.");
            }
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
        UpdateTimerUI();

        if (_remainingTime <= 0f)
        {
            // 시간이 0 이하가 되면 게임 오버 처리
            GameOver();
            return;
        }

        // 플레이어 캐릭터의 Y 위치 체크
        if (_player != null)
        {
            if (_player.transform.position.y < _gridYLowerBound)
            {
                // 플레이어가 낙사했을 경우 게임 오버 처리
                GameOver();
                return;
            }
        }
    }
    public void Exit()
    {
        // (임시) 현재 씬을 다시 로드합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    private void UpdateTimerUI()
    {
        if (_timerText != null)
        {
            // 타이머 UI 업데이트
            int minutes = Mathf.FloorToInt(_remainingTime / 60f);
            int seconds = Mathf.FloorToInt(_remainingTime % 60f);
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void GameOver()
    {
        _isStarted = false;
        // 업데이트를 멈추고 게임 오버 처리
        Exit();
    }

}
