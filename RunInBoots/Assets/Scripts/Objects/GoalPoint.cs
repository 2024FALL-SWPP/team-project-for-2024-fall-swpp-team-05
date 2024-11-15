using UnityEngine;

public class GoalPoint : InteractableObject
{
    protected override void OnInteract()
    {
        GameManager.Instance.GameClear();
        LoadScene(targetStage, targetIndex);
        Debug.Log("Loading next stage: " + targetStage + " index: " + targetIndex);
    }
}
