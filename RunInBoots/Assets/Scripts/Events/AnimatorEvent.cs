using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : ProducingEvent
{
    public AnimatorEvent(Animator animator)
    {
        AddStartEvent(() =>
        {
            Debug.Log("AnimatorEvent Start");
        });
        AddUpdateEvent(() =>
        {
            if (animator == null || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                SetEnd();
            }
        });
        AddEndEvent(() =>
        {
            Debug.Log("AnimatorEvent End");
        });
    }
}
