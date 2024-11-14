using UnityEngine;

public class GoalPoint : InteractableObject
{
    protected override void OnInteract()
    {
        LoadScene(targetStage, targetIndex);
        Debug.Log("Loading next stage: " + targetStage + " index: " + targetIndex);
    }
}
