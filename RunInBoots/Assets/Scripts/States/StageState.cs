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
                Debug.LogWarning("Player�� ã�� �� �����ϴ�.");
            }
            GameObject timerObject = GameObject.Find("TimerText"); // Ÿ�̸� UI �ؽ�Ʈ�� �̸��� "TimerText"��� ����
            if (timerObject != null)
            {
                Debug.Log("Find TimerText");
                _timerText = timerObject.GetComponent<TextMeshProUGUI>();
                UpdateTimerUI();
            }
            else
            {
                Debug.LogWarning("TimerText UI ��Ҹ� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerCamera�� ã�� �� �����ϴ�.");
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
            // �ð��� 0 ���ϰ� �Ǹ� ���� ���� ó��
            GameOver();
            return;
        }

        // �÷��̾� ĳ������ Y ��ġ üũ
        if (_player != null)
        {
            if (_player.transform.position.y < _gridYLowerBound)
            {
                // �÷��̾ �������� ��� ���� ���� ó��
                GameOver();
                return;
            }
        }
    }
    public void Exit()
    {
        // (�ӽ�) ���� ���� �ٽ� �ε��մϴ�.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    private void UpdateTimerUI()
    {
        if (_timerText != null)
        {
            // Ÿ�̸� UI ������Ʈ
            int minutes = Mathf.FloorToInt(_remainingTime / 60f);
            int seconds = Mathf.FloorToInt(_remainingTime % 60f);
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void GameOver()
    {
        _isStarted = false;
        // ������Ʈ�� ���߰� ���� ���� ó��
        Exit();
    }

}
