using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEvent : ProducingEvent
{
    public DelayEvent(float duration)
    {
        AddStartEvent(() =>
        {
            Debug.Log("DelayEvent Start");
        });

        AddUpdateEvent(() =>
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                SetEnd();
            }
        });

        AddEndEvent(() =>
        {
            Debug.Log("DelayEvent End");
        });
    }
}
