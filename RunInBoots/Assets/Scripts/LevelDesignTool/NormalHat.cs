using UnityEngine;

public class NormalHat : Hat
{
    public NormalHat(SkinnedMeshRenderer hatRenderer): base(hatRenderer)
    {
        // call parent constructor
        Debug.Log("Normal Hat created");
    }


    public override void OnEquip()
    {
        hatRenderer.enabled = true;
        Debug.Log("Normal Hat equipped");
    }

    public override void OnUnequip()
    {
        hatRenderer.enabled = false;
        Debug.Log("Normal Hat unequipped");
    }
}