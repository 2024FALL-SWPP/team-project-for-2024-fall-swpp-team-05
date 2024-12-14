using UnityEngine;

public class GoalPoint : Interactable
{
    public override void Initialize()
    {
        //gameObject.name = "AGoalPoint";
        // throw new System.NotImplementedException();
    }

    protected override void OnInteract(GameObject interactor)
    {
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();

        Animator playerAnimator = player.GetComponent<AnimatableUI>().animator;
        player.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_STAGE_CLEAR);
        ProducingEvent stageClearEvent = new AnimatorEvent(playerAnimator);

        stageClearEvent.AddStartEvent(() =>
        {
            // Debug.Log("Stage Clear Event Start");
            actionSystem.ResumeSelf(false);
            transform.position = player.transform.position + Vector3.up * 3.0f;
        });
        stageClearEvent.AddEndEvent(() =>
        {
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
