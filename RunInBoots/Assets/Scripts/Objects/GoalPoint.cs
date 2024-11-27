using UnityEngine;

public class GoalPoint : InteractableObject
{
    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnInteract()
    {
        GameManager.Instance.StageClear();
    }
}
