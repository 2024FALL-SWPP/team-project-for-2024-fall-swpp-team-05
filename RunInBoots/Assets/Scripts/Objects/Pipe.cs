using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Pipe : InteractableObject
{
    public int pipeID;
    public int targetPipeID;

    public override void Initialize()
    {
        SpawnPlayerByPipe();
    }

    public void SpawnPlayerByPipe()
    {
        StageState currentStageState = GameManager.Instance.GetCurrentStageState();
        if (currentStageState.isComingFromPipe && currentStageState.enteredPipeID == pipeID)
        {
            Vector3 targetPosition = transform.position + Vector3.right * 1.5f;
            currentStageState.SpawnPlayer(targetPosition);
            currentStageState.UpdateRespawnPosition(targetPosition);
            currentStageState.ResetPipeData();
        }
    }
    
    protected override void OnInteract()
    {
        int currentIndex = GameManager.Instance.GetCurrentStageState().currentIndex;

        if (currentIndex != targetIndex)
        {
            StageState currentStageState = GameManager.Instance.GetCurrentStageState();
            currentStageState.enteredPipeID = targetPipeID;
            currentStageState.isComingFromPipe = true;
            currentStageState.GoTargetIndexByPipe(targetIndex);
        }
        else
        {
            Pipe targetPipe = PipeUtils.FindPipeByID(targetPipeID);
            if (targetPipe != null)
            {
                targetPipe.GetComponent<Pipe>().Initialize();
            }
            else
            {
                Debug.LogError($"ID가 {targetPipeID}인 파이프를 찾을 수 없습니다.");
            }
        }
    }
}
