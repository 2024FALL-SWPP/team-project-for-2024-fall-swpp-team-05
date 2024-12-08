using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatableUI : MonoBehaviour
{
    public Animator animator;
    
    public void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.CrossFadeInFixedTime(animationName, 0f);
        }
    }
}
