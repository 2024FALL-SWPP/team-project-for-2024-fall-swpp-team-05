using UnityEngine;
using System.Collections.Generic;

public class TigerHat : Hat
{
    public float detectionRadius = 5.0f;
    
    private List<ActionSystem> _scaredDogs = new List<ActionSystem>();

    public TigerHat(SkinnedMeshRenderer hatRenderer): base(hatRenderer)
    {
        // call parent constructor
        Debug.Log("Tiger Hat created");
    }

    public override void OnEquip()
    {
        hatRenderer.enabled = true;
    }

    public override void OnUnequip()
    {
        hatRenderer.enabled = false;
        ResetScaredDogs(); // 호랑이 모자의 특별한 행동 초기화
    }

    public void HandleTigerBehavior(Transform transform)
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        HashSet<ActionSystem> currentFrameDogs = new HashSet<ActionSystem>();

        foreach (var collider in nearbyColliders)
        {
            ActionSystem ActionSystem = collider.GetComponent<ActionSystem>();
            if (ActionSystem != null && ActionSystem.currentAction.Key == 601)
            {
                ActionSystem.SetAction(602); // 도망 행동
                currentFrameDogs.Add(ActionSystem);

                if (!_scaredDogs.Contains(ActionSystem))
                {
                    _scaredDogs.Add(ActionSystem);
                }
            }
        }

        // 범위를 벗어난 개들 처리
        for (int i = _scaredDogs.Count - 1; i >= 0; i--)
        {
            if (!currentFrameDogs.Contains(_scaredDogs[i]))
            {
                _scaredDogs[i].SetAction(601); // 걷기 복귀
                _scaredDogs.RemoveAt(i);
            }
        }
    }

    private void ResetScaredDogs()
    {
        foreach (var dog in _scaredDogs)
        {
            dog.SetAction(601); // 걷기 복귀
        }
        _scaredDogs.Clear();
    }
}