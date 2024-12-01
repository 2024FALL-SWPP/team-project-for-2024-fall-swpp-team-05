using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Pipe : Interactable
{
    public int targetIndex;
    public int pipeID;
    public int targetPipeID;

    public override void Initialize()
    {
        SpawnPlayerByPipe();
    }

    public void SpawnPlayerByPipe()
    {
        StageState currentStageState = GameManager.Instance.GetCurrentStageState();
        Vector3 targetPosition = transform.position + Vector3.right * 1.5f;
        currentStageState.SpawnPlayer(targetPosition);
        currentStageState.UpdateRespawnPosition(targetPosition, false);
    }
    
    protected override void OnInteract(GameObject interactor)
    {
        StageState currentStageState = GameManager.Instance.GetCurrentStageState();
        int currentIndex = GameManager.Instance.GetCurrentStageState().currentIndex;
        
        if (currentStageState == null)
        {
            Debug.LogWarning("현재 Stage State가 아님");
            return;
        }
        
        if (currentIndex != targetIndex)
        {
            currentStageState.GoTargetIndexByPipe(targetIndex, targetPipeID);
        }
        else
        {
            Pipe targetPipe = PipeUtils.FindPipeByID(targetPipeID);
            targetPipe?.Initialize();
        }
    }
}
