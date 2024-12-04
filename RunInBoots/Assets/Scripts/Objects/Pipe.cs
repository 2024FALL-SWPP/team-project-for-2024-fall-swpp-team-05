using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

public class Pipe : Interactable
{
    public int targetIndex;
    public int pipeID;
    public int targetPipeID;

    private Collider pipeCollider;


    public override void Initialize()
    {
        SpawnPlayerByPipe();
    }

    public void SpawnPlayerByPipe()
    {
        pipeCollider = GetComponent<Collider>();
        StartCoroutine(DisableCollisionTemporarily());

        StageState currentStageState = GameManager.Instance.GetCurrentStageState();
        Vector3 targetPosition = transform.position;
        currentStageState.SpawnPlayer(targetPosition);
        currentStageState.UpdateRespawnPosition(targetPosition, false);
    }

    private IEnumerator DisableCollisionTemporarily()
    {
        if (pipeCollider != null)
        {
            pipeCollider.enabled = false; // Collider 비활성화
            yield return new WaitForSeconds(1f); // 1초 대기
            pipeCollider.enabled = true; // Collider 다시 활성화
        }
    }

    protected override void OnInteract(GameObject interactor)
    {
        ProducingEvent pipeInteractionEvent = new AnimatorEvent(null);
        ProducingEvent blackScreenEvent = new AnimatorEvent(null);
        ProducingEvent pipeOpeningEvent = new AnimatorEvent(null);
        pipeInteractionEvent.AddStartEvent(() =>
        {
            GameManager.Instance.StopPlayer();
        });
        blackScreenEvent.AddEndEvent(() => {
            OnPipeInteraction(interactor);
        });
        pipeOpeningEvent.AddEndEvent(() =>
        {
            GameManager.Instance.ResumePlayer();
            GameManager.Instance.MakePlayerInvincible();
        });
        GameManager.Instance.AddEvent(pipeInteractionEvent);
        GameManager.Instance.AddEvent(blackScreenEvent);
        GameManager.Instance.AddEvent(pipeOpeningEvent);
    }

    private void OnPipeInteraction(GameObject interactor)
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
