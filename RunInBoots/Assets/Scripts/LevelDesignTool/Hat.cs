using UnityEngine;

public abstract class Hat
{
    public eHatType HatType;
    public SkinnedMeshRenderer hatRenderer;

    public Hat(SkinnedMeshRenderer hatRenderer)
    {
        this.hatRenderer = hatRenderer;
    }

    public abstract void OnEquip();
    public abstract void OnUnequip();
}