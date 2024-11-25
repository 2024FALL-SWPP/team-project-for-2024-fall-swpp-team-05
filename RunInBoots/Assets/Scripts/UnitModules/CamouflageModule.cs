using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamouflageModule : MonoBehaviour
{
    public SkinnedMeshRenderer normalHatRenderer, tigerHatRenderer; // 모자 렌더러
    
    private eHatType _currentHatType = eHatType.None;
    private Hat _currentHat; // 현재 장착된 모자 객체
    private BattleModule _battleModule; // 체력 증감을 위한 BattleModule 참조

    public void Start()
    {
        // 모든 모자 비활성화
        normalHatRenderer.enabled = false;
        tigerHatRenderer.enabled = false;

        // initialize battle module
        _battleModule = GetComponent<BattleModule>();
        _battleModule.preAttacked.AddListener(UnequipHat);
    }

    public void EquipHat(eHatType hatType)
    {   
        if ((int)hatType <= (int)_currentHatType)
        {
            Debug.Log($"Hat {hatType} not equipped: priority too low.");
            return; // 우선순위가 낮으면 무시
        }

        // 현재 모자가 있다면 탈착
        if (_currentHat != null)
        {
            UnequipCurrentHat();
        }

        // 새 모자 장착
        if (hatType == eHatType.Normal)
        {
            _currentHat = new NormalHat(normalHatRenderer);
        }
        else if (hatType == eHatType.Tiger)
        {
            _currentHat = new TigerHat(tigerHatRenderer);
        }


        _currentHatType = hatType;
        _currentHat.OnEquip();
        _battleModule.health += 1; // 체력 증가
        Debug.Log($"Hat {_currentHat} equipped successfully.");
    }

    public void UnequipHat()
    {
        if (_currentHatType == eHatType.None || _currentHat == null)
        {
            Debug.Log("No hat to unequip.");
            return;
        }

        // 현재 모자 탈착
        UnequipCurrentHat();
        _currentHatType = eHatType.None;
        _currentHat = null;

        Debug.Log("Hat unequipped.");
    }

    private void UnequipCurrentHat()
    {
        if (_currentHat != null)
        {
            _currentHat.OnUnequip();
            Debug.Log($"Hat {_currentHat} unequipped.");
        }
    }

    // update
    public void Update()
    {
        if (_currentHatType == eHatType.Tiger)
        {
            TigerHat tigerHat = (TigerHat)_currentHat;
            tigerHat.HandleTigerBehavior(transform);
        }
    }
}