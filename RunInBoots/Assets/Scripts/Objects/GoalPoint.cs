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
        GameManager.Instance.GetCurrentStageState().Exit(ExitState.StageClear);
    }
}
