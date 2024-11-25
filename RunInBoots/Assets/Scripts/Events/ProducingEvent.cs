using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProducingEvent
{
    public Action startEvent { get; private set; }
    public Action updateEvent { get; private set; }
    public Action endEvent { get; private set; }
    public bool isEnded { get; private set; }

    public ProducingEvent()
    {
        isEnded = false;
    }

    public void Start()
    {
        startEvent?.Invoke();
    }

    public void Update()
    {
        updateEvent?.Invoke();
    }

    public void Exit()
    {
        endEvent?.Invoke();
    }

    public void SetEnd()
    {
        isEnded = true;
    }
    
    public void AddStartEvent(Action action)
    {
        startEvent += action;
    }

    public void AddUpdateEvent(Action action)
    {
        updateEvent += action;
    }

    public void AddEndEvent(Action action)
    {
        endEvent += action;
    }
}
