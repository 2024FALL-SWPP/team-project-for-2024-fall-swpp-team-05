using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class AttackEffect : MonoBehaviour
{
    public UnityEvent OnActive;
    private float _time = 0.0f;
    // Start is called before the first frame update
    void OnEnable()
    {
        // time reset
        _time = 0.0f;
        // play visual effect
        OnActive.Invoke();
        Debug.Log("AttackEffect Awake");
    }

    // Update is called once per frame
    void Update()
    {
        // set active false after 0.8f
        if(_time > 0.8f)
        {
            gameObject.SetActive(false);
            return;
        }
        _time += Time.deltaTime;
    }
}
