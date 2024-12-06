using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CamouflageModule : MonoBehaviour
{
    public SkinnedMeshRenderer normalHatRenderer, tigerHatRenderer; // 모자 렌더러
    
    private eHatType _currentHatType = eHatType.None;
    private Hat _currentHat; // 현재 장착된 모자 객체
    private BattleModule _battleModule; // 체력 증감을 위한 BattleModule 참조
    public UnityEvent onChangeHat; // 모자 추적 이벤트

    public void Initialize(eHatType type)
    {
        // initialize hat renderers
        InitializeHatRenderers(type);
        EquipHat(type);
    }

    public void InitializeBattleModule()
    {
        _battleModule = GetComponent<BattleModule>();
        _battleModule.preAttacked.AddListener(UnequipHat);
    }

    public void InitializeHatRenderers(eHatType type)
    {
        normalHatRenderer = GameObject.Find("NormalHat").GetComponent<SkinnedMeshRenderer>();
        tigerHatRenderer = GameObject.Find("TigerHat").GetComponent<SkinnedMeshRenderer>();

        if (type == eHatType.Normal)
        {
            normalHatRenderer.enabled = true;
            tigerHatRenderer.enabled = false;
        }
        else if (type == eHatType.Tiger)
        {
            normalHatRenderer.enabled = false;
            tigerHatRenderer.enabled = true;
        }
        else {
            normalHatRenderer.enabled = false;
            tigerHatRenderer.enabled = false;
        }
        Debug.Log("Hat renderers initialized.");
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
        else {
            _battleModule.health += 1; // 체력 증가
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
        Debug.Log($"Hat {_currentHat} equipped successfully.");
        onChangeHat.Invoke();
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
        onChangeHat.Invoke();
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

    public eHatType GetCurrentHatType()
    {
        return _currentHatType;
    }

    public void SetBattleModule(BattleModule battleModule)
    {
        _battleModule = battleModule;
    }

    public BattleModule GetBattleModule()
    {
        return _battleModule;
    }
}