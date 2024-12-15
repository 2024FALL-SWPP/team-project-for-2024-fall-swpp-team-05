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
    private GameObject player;


    public override void Initialize()
    {
        SpawnPlayerByPipe();
    }

    public void SpawnPlayerByPipe()
    {
        StageState currentStageState = GameManager.Instance.GetCurrentStageState();
        Vector3 targetPosition = transform.position;
        currentStageState.SpawnPlayer(targetPosition);
        currentStageState.UpdateRespawnPosition(targetPosition, false);

        Animator pipeAnimator = GetComponent<Animator>();
        pipeAnimator.CrossFadeInFixedTime(UIConst.ANIM_PIPE_OPENING, 0.0f);
    }

    public IEnumerator DisableCollisionTemporarily()
    {
        pipeCollider = GetComponent<Collider>();
        if (pipeCollider != null)
        {
            Debug.LogWarning($"Disabling Pipe {pipeID} Collider...");
            pipeCollider.enabled = false; // Collider 비활성화
            yield return new WaitForSeconds(3f); // 1초 대기
            pipeCollider.enabled = true; // Collider 다시 활성화
            Debug.LogWarning($"Pipe {pipeID} Collider reenabled.");
        }
    }

    protected override void OnInteract(GameObject interactor)
    {
        player = interactor;
        ActionSystem actionSystem = player.GetComponent<ActionSystem>();
        BattleModule battleModule = player.GetComponent<BattleModule>();

        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        GameObject blackScreen = Resources.Load<GameObject>("BlackScreenUI");
        Animator pipeAnimator = GetComponent<Animator>();

        ProducingEvent pipeInteractionEvent = new AnimatorEvent(pipeAnimator);
        pipeAnimator.CrossFadeInFixedTime(UIConst.ANIM_PIPE_INTERACTION, 0.0f);
        pipeInteractionEvent.AddStartEvent(() =>
        {
            actionSystem.ResumeSelf(false);
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
                //for scene change
                if (GameManager.Instance.GetCurrentStageState().currentIndex != targetIndex)
                    SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => OnPipeEnter(scene, mode);
                //for same scene
                else
                {
                    OnPipeEnter(SceneManager.GetActiveScene(), LoadSceneMode.Single);
                    screenAnimator.gameObject.SetActive(false);
                }
                    
            });
            GameManager.Instance.AddEvent(blackScreenEvent);
        });
        GameManager.Instance.AddEvent(pipeInteractionEvent);
    }

    private void OnPipeEnter(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Pipe Enter Event");
        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        GameObject blackScreen = Resources.Load<GameObject>("BlackScreenUI");
        GameObject pipeUI = PoolManager.Instance.Pool(blackScreen, Vector3.zero, Quaternion.identity, canvas.transform);
        pipeUI.transform.SetParent(canvas.transform, false);
        pipeUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        Animator screenAnimator = pipeUI.GetComponent<Animator>();
        pipeUI.GetComponent<AnimatableUI>().PlayAnimation(UIConst.ANIM_BLACK_END);
        ProducingEvent pipeOpeningEvent = new AnimatorEvent(screenAnimator);

        pipeOpeningEvent.AddEndEvent(() =>
        {
            if(player == null)
                player = GameObject.FindWithTag("Player");
            var battleModule = player?.GetComponent<BattleModule>();
            var actionModule = player?.GetComponent<ActionSystem>();
            //resume player
            player?.SetActive(true);
            actionModule?.ResumeSelf(true);
            battleModule?.BeInvincible();
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
