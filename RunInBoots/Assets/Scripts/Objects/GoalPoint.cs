using UnityEngine;

public class GoalPoint : Interactable
{
    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnInteract(GameObject interactor)
    {
        GameManager.Instance.GetCurrentStageState().Exit(ExitState.StageClear);
    }
}
