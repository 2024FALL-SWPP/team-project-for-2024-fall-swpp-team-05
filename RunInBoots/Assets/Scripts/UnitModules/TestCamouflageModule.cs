using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class TestCamouflageModule
{
    public static void Test()
    {
        Debug.Log("Test CamouflageModule");

        var go = new GameObject();
        go.AddComponent<CamouflageModule>();
        var camouflageModule = go.GetComponent<CamouflageModule>();

        // stub renderer
        camouflageModule.normalHatRenderer = go.AddComponent<SkinnedMeshRenderer>();
        camouflageModule.tigerHatRenderer = camouflageModule.normalHatRenderer;
        Debug.Assert(camouflageModule.normalHatRenderer != null);
        // stub battle module
        var battleModule = go.AddComponent<BattleModule>();
        var initialBattleModuleHealth = battleModule.health;
        camouflageModule.SetBattleModule(battleModule);

        camouflageModule.EquipHat(eHatType.Normal);
        // check _currentHat is NormalHat
        Debug.Assert(camouflageModule.GetCurrentHatType() == eHatType.Normal);

        camouflageModule.EquipHat(eHatType.Tiger);
        // check _currentHat is TigerHat
        Debug.Assert(camouflageModule.GetCurrentHatType() == eHatType.Tiger);

        camouflageModule.EquipHat(eHatType.Normal);
        // check _currentHat is TigerHat (still)
        Debug.Assert(camouflageModule.GetCurrentHatType() == eHatType.Tiger);

        // check health is increased by 1
        Debug.Assert(camouflageModule.GetBattleModule().health == initialBattleModuleHealth + 1);

        camouflageModule.UnequipHat();
        // check current hat type is eHatType.None
        Debug.Assert(camouflageModule.GetCurrentHatType() == eHatType.None);

        
        // remove all components
        Object.Destroy(go);
    }
}