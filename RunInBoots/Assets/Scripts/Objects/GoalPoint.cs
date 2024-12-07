using UnityEngine;

public class GoalPoint : Interactable
{
    public override void Initialize()
    {
        gameObject.name = "AGoalPoint";
        // throw new System.NotImplementedException();
    }

    protected override void OnInteract(GameObject interactor)
    {
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        ProducingEvent stageClearEvent = new AnimatorEvent(null);
        ProducingEvent blackScreenEvent = new AnimatorEvent(null);
        stageClearEvent.AddStartEvent(() =>
        {
            // Debug.Log("Stage Clear Event Start");
            actionSystem.ResumeSelf(false);
            transform.position = player.transform.position + Vector3.up * 3.0f;
        });
        blackScreenEvent.AddEndEvent(() =>
        {
            // Debug.Log("Stage Clear Event End");
            actionSystem.ResumeSelf(true);
            OnGoalReached();
        });
        GameManager.Instance.AddEvent(stageClearEvent);
        GameManager.Instance.AddEvent(blackScreenEvent);
    }

    private void OnGoalReached()
    {
        GameManager.Instance.GetCurrentStageState().Exit(ExitState.StageClear);
    }
}
