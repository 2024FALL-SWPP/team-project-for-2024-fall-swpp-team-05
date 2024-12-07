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

        Animator pipeAnimator = GetComponent<Animator>();
        pipeAnimator.CrossFadeInFixedTime(UIConst.ANIM_PIPE_OPENING, 0.0f);
    }
    
    protected override void OnInteract(GameObject interactor)
    {
        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        BattleModule battleModule = player.GetComponent<BattleModule>();

        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        GameObject blackScreen = Resources.Load<GameObject>("BlackScreenUI");
        Animator pipeAnimator = GetComponent<Animator>();

        ProducingEvent pipeInteractionEvent = new AnimatorEvent(pipeAnimator);
        pipeAnimator.CrossFadeInFixedTime(UIConst.ANIM_PIPE_INTERACTION, 0.0f);
        pipeInteractionEvent.AddStartEvent(() =>
        {
            player.SetActive(false);
        });
        pipeInteractionEvent.AddEndEvent(() =>
        {
            GameObject pipeUI = PoolManager.Instance.Pool(blackScreen, Vector3.zero, Quaternion.identity, canvas.transform);
            pipeUI.transform.SetParent(canvas.transform, false);
            pipeUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            pipeUI.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_BLACK_START);
            
            Animator screenAnimator = pipeUI.GetComponent<Animator>();
            ProducingEvent blackScreenEvent = new AnimatorEvent(screenAnimator);
            blackScreenEvent.AddEndEvent(() => {
                OnPipeInteraction(interactor);
                SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => OnPipeEnter(scene, mode);
            });
            GameManager.Instance.AddEvent(blackScreenEvent);
        });
        GameManager.Instance.AddEvent(pipeInteractionEvent);
    }

    private void OnPipeEnter(Scene scene, LoadSceneMode mode)
    {
        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        GameObject blackScreen = Resources.Load<GameObject>("BlackScreenUI");
        GameObject pipeUI = PoolManager.Instance.Pool(blackScreen, Vector3.zero, Quaternion.identity, canvas.transform);
        pipeUI.transform.SetParent(canvas.transform, false);
        pipeUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        Animator screenAnimator = pipeUI.GetComponent<Animator>();
        pipeUI.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_BLACK_END);
        ProducingEvent pipeOpeningEvent = new AnimatorEvent(screenAnimator);

        GameObject player = GameObject.FindWithTag("Player");
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        BattleModule battleModule = player.GetComponent<BattleModule>();

        pipeOpeningEvent.AddEndEvent(() =>
        {
            if(player == null)
            {
                player = GameObject.FindWithTag("Player");
                actionSystem = player.GetComponent<ActionSystem>();
                battleModule = player.GetComponent<BattleModule>();
            }
            battleModule.BeInvinvible();
        });
        GameManager.Instance.AddEvent(pipeOpeningEvent);
        SceneManager.sceneLoaded -= OnPipeEnter;
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
