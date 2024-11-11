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
    // public Animator animator;

    [SerializeField] private ActionTableEntity currentAction;
    private FrameUpdateRule[] frameUpdates;
    private int actionFrames = 0;

    private TransformModule transformModule;
    private BattleModule battleModule;
    private BoxCollider coll;

    #region Functions for setting new action
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
        Debug.Log("Change action: " + nextAction);

        currentAction = actions.Actions.Find(x => x.Key == nextAction);
        string updates = currentAction.FrameUpdates;
        ParseUpdateRules(updates);
        actionFrames = 0;
        transformModule.g_scale = currentAction.GravityScale;
        transformModule.maxSpeedX = currentAction.MaxVelocityX;
        transformModule.maxSpeedY = currentAction.MaxVelocityY;
        // animator.CrossFade(currentAction.Clip, currentAction.TransitionDuration);
    }
    #endregion

    #region Functions for checking conditions
    bool VerticalSpaceCheck()
    {
        // Check if there is enough space above the character
        Vector3 origin = transform.position;
        origin.y += coll.size.y;
        RaycastHit hit;
        float distance = coll.size.y;
        if(Physics.Raycast(origin, Vector3.up, out hit, distance))
        {
            // Check if there is enough space under the character
            origin = transform.position;
            if(Physics.Raycast(origin, Vector3.down, out hit, distance))
            {
                return false;
            }
        }
        
        return true;
    }

    bool CheckOnLand()
    {
        // Check if character is on the ground
        Vector3 origin = transform.position;
        origin.y += coll.size.y / 2;
        RaycastHit hit;
        float distance = 1 + coll.size.y / 2;
        if(Physics.Raycast(origin, Vector3.down, out hit, distance) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            return true;
        }
        return false;
    }

    bool CheckOnWall()
    {
        // Check if character is on the wall
        Vector3 origin = transform.position;
        RaycastHit hit;
        float distance = 1 + coll.size.x / 2;
        if(Physics.Raycast(origin, Vector3.right, out hit, distance) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            return true;
        }

        if(Physics.Raycast(origin, Vector3.left, out hit, distance) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            return true;
        }
        return false;
    }

    bool CheckPlayerInSight()
    {
        // Check if player is in sight
        Collider[] colliders = Physics.OverlapSphere(transform.position, 7.0f);
        foreach(Collider col in colliders)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    bool CheckWalkable()
    {
        Vector3 direction = transform.forward;
        if(direction.y == 0) direction = Vector3.right;
        else direction = Vector3.left;

        Vector3 origin = transform.position;

        float distance = 1 + coll.size.x / 2;
        RaycastHit hit;

        if(Physics.Raycast(origin, direction, out hit, distance))
        {
            BattleModule unit = hit.collider.gameObject.GetComponent<BattleModule>();
            if(unit != null && unit.team == battleModule.team)
            {
                return false;
            }
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return false;
            }
        }

        // Check if there is a hole in front of the character
        origin = transform.position;
        if(direction.y == 0) origin.x += 1.0f;
        else origin.x -= 1.0f;
        origin.y += coll.size.y / 2;
        distance = 1 + coll.size.y / 2;
        direction = Vector3.down;
        hit = new RaycastHit();

        if(Physics.Raycast(origin, direction, out hit, distance))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return true;
            }
        }
        return false;
    }

    bool CheckCondition(eActionCondition cond, int val)
    {
        int cond_val = 0;
        switch(cond) {
            case eActionCondition.InputX: 
                if(Input.GetAxis("Horizontal") > 0) cond_val = 1;
                else if(Input.GetAxis("Horizontal") < 0) cond_val = -1;
                else cond_val = 0;
                break;
            case eActionCondition.InputY: 
                if(Input.GetAxis("Vertical") > 0) cond_val = 1;
                else if(Input.GetAxis("Vertical") < 0) cond_val = -1;
                else cond_val = 0;
                break;
            case eActionCondition.Risable: 
                if(Input.GetAxis("Vertical") >= 0 && VerticalSpaceCheck()) cond_val = 1;
                else cond_val = 0;
                break;
            case eActionCondition.Jump: 
                if(Input.GetKeyDown(KeyCode.X)) cond_val = 1;
                else cond_val = 0;
                break;
            case eActionCondition.JumpValid: 
                if(transformModule.jumpAllowed && Input.GetKeyDown(KeyCode.X)) cond_val = 1;
                else cond_val = 0;
                break;
            case eActionCondition.Attack: 
                if(Input.GetKeyDown(KeyCode.Z)) cond_val = 1;
                else cond_val = 0;
                break;
            case eActionCondition.Run: 
                if(Input.GetKeyDown(KeyCode.Z)) {
                    if(Input.GetAxis("Horizontal") != 0) cond_val = 1;
                    else cond_val = 0;
                }
                else cond_val = 0;
                break;
            case eActionCondition.OnLand: 
                if(CheckOnLand()) cond_val = 1;
                else cond_val = 0;
                break;
            case eActionCondition.OnWall:
                if(CheckOnWall()) cond_val = 1;
                else cond_val = 0;
                break;
            case eActionCondition.ShrinkEnd:
                if(coll.size.x == currentAction.ColliderX && coll.size.y == currentAction.ColliderY) cond_val = 1;
                else cond_val = 0;
                break;
            case eActionCondition.Frame: 
                cond_val = actionFrames;
                break;
            case eActionCondition.Walkable:
                if(CheckWalkable()) cond_val = 1;
                else cond_val = 0;
                break;
            case eActionCondition.PlayerInSight:
                if(CheckPlayerInSight()) cond_val = 1;
                else cond_val = 0;
                break;

        }
        if(cond_val == val) Debug.Log("Check condition: " + cond + " " + val);
        return (cond_val == val);
    }
    #endregion

    void RunFunction(eActionFunction func, float val)
    {
        // Run function
        Debug.Log("Run function: " + func + " " + val);
    }

    // Start is called before the first frame update
    void Start()
    {
        transformModule = GetComponent<TransformModule>();
        battleModule = GetComponent<BattleModule>();
        coll = GetComponent<BoxCollider>();
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
                    break;
                }
            }
        }
        // if current clip is not looping and animation is finished, set next action
        if(currentAction.NextAction != 0 && actionFrames == 10) // && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
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
