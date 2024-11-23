using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� ��ũ��Ʈ�� �� ���� ���� ������ �����ߴ� ������Ʈ���� ���������� �˸�.

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
