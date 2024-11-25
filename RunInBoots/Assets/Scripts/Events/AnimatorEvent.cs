using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : ProducingEvent
{
    public AnimatorEvent(Animator animator)
    {
        AddUpdateEvent(() =>
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                SetEnd();
            }
        });
    }
}
