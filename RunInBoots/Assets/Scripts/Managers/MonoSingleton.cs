using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 코드는 이전에 개인이 진행하던 프로젝트에서 가져온 코드임을 알림

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    // Start is called before the first frame update

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>() as T;

                if (_instance == null)
                {
                    Debug.LogWarning("There's no active " + typeof(T) + " in this scene.");
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (Instance != this)
            gameObject.SetActive(false);
        else
            DontDestroyOnLoad(gameObject);
    }
}
