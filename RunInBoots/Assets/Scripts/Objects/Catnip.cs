using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catnip : Interactable
{
    public int catnipID;

    private void Start()
    {
        gameObject.name = "Catnip_" + catnipID;
        Initialize();
    }

    public override void Initialize()
    {
        if (GameManager.Instance.GetCurrentStageState()?.isCatnipCollected[catnipID - 1] ?? false)
        {
            gameObject.SetActive(false);
        }
    }

    protected override void OnInteract(GameObject interactor)
    {
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        BattleModule battleModule = player.GetComponent<BattleModule>();

        Animator playerAnimator = player.GetComponent<AnimatableUI>().animator;
        player.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_PLAYER_CATNIP);
        ProducingEvent catnipEvent = new AnimatorEvent(playerAnimator);
        catnipEvent.AddStartEvent(() =>
        {
            // Debug.Log("Catnip Event Start");
            actionSystem.ResumeSelf(false);
            transform.position = player.transform.position + Vector3.up * 3.0f;
        });
        catnipEvent.AddEndEvent(() =>
        {
            actionSystem.ResumeSelf(true);
            battleModule.BeInvincible();
            OnCatnipCollected();
            // Debug.Log("Catnip Event End");
        });
        GameManager.Instance.AddEvent(catnipEvent);
    }

    private void OnCatnipCollected()
    {
        GameManager.Instance.GetCurrentStageState().CollectCatnipInStageState(catnipID);
        gameObject.SetActive(false);
    }
}
