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
        Vector3 targetPosition = transform.position + Vector3.right * 1.5f;
        currentStageState.SpawnPlayer(targetPosition);
        currentStageState.UpdateRespawnPosition(targetPosition, false);
    }
    
    protected override void OnInteract()
    {
        int currentIndex = GameManager.Instance.GetCurrentStageState().currentIndex;
        StageState currentStageState = GameManager.Instance.GetCurrentStageState();

        if (currentIndex != targetIndex)
        {
            currentStageState.GoTargetIndexByPipe(targetIndex, targetPipeID);
        }
        else
        {
            Pipe targetPipe = PipeUtils.FindPipeByID(targetPipeID);
            Debug.Log($"씬 내 파이프 이동: {pipeID} -> {targetPipeID}");
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
