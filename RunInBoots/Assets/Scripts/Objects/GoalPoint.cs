using UnityEngine;

public class GoalPoint : Interactable
{
    private Animator animator;
    public override void Initialize()
    {
        //gameObject.name = "AGoalPoint";
        // throw new System.NotImplementedException();
        animator = GetComponent<Animator>();
        animator.CrossFadeInFixedTime(UIConst.ANIM_RAT_SAD, 0.0f);
    }

    protected override void OnInteract(GameObject interactor)
    {
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        TransformModule transformModule = player.GetComponent<TransformModule>();

        Animator playerAnimator = player.GetComponent<AnimatableUI>().animator;
        player.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_STAGE_CLEAR);
        ProducingEvent stageClearEvent = new AnimatorEvent(playerAnimator);

        stageClearEvent.AddStartEvent(() =>
        {
            if(animator == null) animator = GetComponent<Animator>();
            animator.CrossFadeInFixedTime(UIConst.ANIM_RAT_TRAN, 0.0f);
            // Debug.Log("Stage Clear Event Start");
            transformModule.LookAhead();
            actionSystem.ResumeSelf(false);
            transform.position = player.transform.position + Vector3.up * 4.0f;
        });
        stageClearEvent.AddEndEvent(() =>
        {
            if(animator == null) animator = GetComponent<Animator>();
            animator.CrossFadeInFixedTime(UIConst.ANIM_RAT_HAPPY, 0.0f);
            GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
            GameObject blackScreen = Resources.Load<GameObject>("ClearUI");
            GameObject blackScreenObj = PoolManager.Instance.Pool(blackScreen, Vector3.zero, Quaternion.identity, canvas.transform);
            blackScreenObj.transform.SetParent(canvas.transform, false);
            blackScreenObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            Animator screenAnimator = blackScreenObj.GetComponent<Animator>();
            blackScreenObj.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_CLEAR_UI);
            ProducingEvent blackScreenEvent = new AnimatorEvent(screenAnimator);
            blackScreenEvent.AddEndEvent(() =>
            {
                // Debug.Log("Stage Clear Event End");
                actionSystem.ResumeSelf(true);
                OnGoalReached();
            });
            GameManager.Instance.AddEvent(blackScreenEvent);
        });
        GameManager.Instance.AddEvent(stageClearEvent);
    }

    private void OnGoalReached()
    {
        GameManager.Instance.GetCurrentStageState().Exit(eExitState.StageClear);
    }
}
