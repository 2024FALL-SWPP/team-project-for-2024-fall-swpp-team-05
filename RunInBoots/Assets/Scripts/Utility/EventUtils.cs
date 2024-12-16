using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EventUtils
{
    public static ProducingEvent SpawnEvent(int currentStage, int lifeCount, Vector3 spawnPosition)
    {
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player?.GetComponent<ActionSystem>();
        GameObject spawnUI = Resources.Load<GameObject>("SpawnUI");
        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        SpawnUI text = PoolManager.Instance.Pool(spawnUI, Vector3.zero, Quaternion.identity, canvas.transform).GetComponent<SpawnUI>();
        text.transform.SetParent(canvas.transform, false);
        text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        text.UpdateStageText(currentStage);
        text.UpdateLifeContainer(lifeCount);
        Animator spawnAnimator = text.GetComponent<Animator>();
        ProducingEvent spawnEvent = new AnimatorEvent(spawnAnimator);
        spawnEvent.AddStartEvent(() =>
        {
            if (player == null || actionSystem == null)
            {
                player = GameObject.FindWithTag("Player");
                actionSystem = player.GetComponent<ActionSystem>();
                actionSystem.ResumeSelf(false);
            }
            else actionSystem.ResumeSelf(false);
            //set idle and grounded
            actionSystem.SetAction(actionSystem.initAction);
            //ray cast ground beneath the spawn position
            if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                player.transform.position = hit.point;
            }
        });
        spawnEvent.AddEndEvent(() =>
        {
            if (player == null || actionSystem == null)
            {
                player = GameObject.FindWithTag("Player");
                actionSystem = player.GetComponent<ActionSystem>();
                actionSystem.ResumeSelf(true);
            }
            else actionSystem.ResumeSelf(true);
        });
        return spawnEvent;
    }
    public static ProducingEvent BlackScreenEvent()
    {
        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        GameObject blackScreen = Resources.Load<GameObject>("BlackScreenUI");
        GameObject blackScreenObj = PoolManager.Instance.Pool(blackScreen, Vector3.zero, Quaternion.identity, canvas.transform);
        blackScreenObj.transform.SetParent(canvas.transform, false);
        blackScreenObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        Animator screenAnimator = blackScreenObj.GetComponent<Animator>();
        blackScreenObj.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_BLACK_START);
        ProducingEvent blackScreenEvent = new AnimatorEvent(screenAnimator);
        return blackScreenEvent;
    }
    public static ProducingEvent DeathEvent()
    {
        Debug.Log("LifeOverWithEvent");
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        TransformModule transformModule = player.GetComponent<TransformModule>();
        Animator playerAnimator = player.GetComponent<AnimatableUI>().animator;
        player.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_PLAYER_DEATH);
        ProducingEvent deathEvent = new AnimatorEvent(playerAnimator);
        deathEvent.AddStartEvent(() =>
        {
            Debug.Log("LifeOver Event Start");
            transformModule.LookAhead();
            if (actionSystem != null) actionSystem.ResumeSelf(false);
        });
        return deathEvent;
    }
}