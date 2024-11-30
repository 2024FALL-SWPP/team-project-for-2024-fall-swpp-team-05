using UnityEngine;

public class GoalPoint : InteractableObject
{
    protected override void OnInteract()
    {
        GameManager.Instance.StageClearWithEvent();
    }
}
