using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FrameUpdateRule
{
    public eActionCondition cond_name;
    public int cond_value;
    public eActionFunction func_name;
    public float func_value;
}

public class ActionSystem : MonoBehaviour
{
    public ActionTable actions;
    public int initAction;
    public Animator animator;

    private ActionTableEntity currentAction;
    private FrameUpdateRule[] frameUpdates;
    private int actionFrames = 0;

    private TransformModule transformModule;

    void ParseUpdateRules(string updates)
    {
        updates = updates.Substring(2, updates.Length - 4);
        updates = updates.Replace("},{", ":");
        string[] updateArray = updates.Split(':').ToArray();
        frameUpdates = new FrameUpdateRule[updateArray.Length];
        for(int i = 0; i < updateArray.Length; i++)
        {
            string[] update = updateArray[i].Split(',');
            frameUpdates[i] = new FrameUpdateRule();
            frameUpdates[i].cond_name = (eActionCondition)Enum.Parse(typeof(eActionCondition), update[0]);
            frameUpdates[i].cond_value = int.Parse(update[1]);
            frameUpdates[i].func_name = (eActionFunction)Enum.Parse(typeof(eActionFunction), update[2]);
            frameUpdates[i].func_value = float.Parse(update[3]);
        }
    }

    public void SetAction(int nextAction)
    {
        currentAction = actions.Actions.Find(x => x.Key == nextAction);
        string updates = currentAction.FrameUpdates;
        ParseUpdateRules(updates);
        actionFrames = 0;
        transformModule.g_scale = currentAction.GravityScale;
        transformModule.maxSpeedX = currentAction.MaxVelocityX;
        transformModule.maxSpeedY = currentAction.MaxVelocityY;
        animator.CrossFade(currentAction.Clip, currentAction.TransitionDuration);

        Debug.Log("Change action: " + currentAction.Key);
    }

    bool CheckCondition(eActionCondition cond, int val)
    {
        Debug.Log("Check condition: " + cond + " " + val);
        return true;
    }

    void RunFunction(eActionFunction func, float val)
    {
        // Run function
        Debug.Log("Run function: " + func + " " + val);
    }

    // Start is called before the first frame update
    void Start()
    {
        transformModule = GetComponent<TransformModule>();
        SetAction(initAction);
    }

    // Update is called once per frame
    void Update()
    {
        // Check all frame updates if func_value is SetAction
        foreach(FrameUpdateRule rule in frameUpdates)
        {
            if(rule.func_name == eActionFunction.SetAction)
            {
                // Check if condition is met 
                if(CheckCondition(rule.cond_name, rule.cond_value))
                {
                    SetAction((int)rule.func_value);
                    return;
                }
            }
        }
        // if current clip is not looping and animation is finished, set next action
        if(currentAction.NextAction != 0 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            SetAction(currentAction.NextAction);
            return;
        }
        // Check all frame updates if func_value is not SetAction
        foreach(FrameUpdateRule rule in frameUpdates)
        {
            if(rule.func_name != eActionFunction.SetAction)
            {
                if(CheckCondition(rule.cond_name, rule.cond_value))
                {
                    RunFunction(rule.func_name, rule.func_value);
                }
            }
        }
        actionFrames++;
    }
}
