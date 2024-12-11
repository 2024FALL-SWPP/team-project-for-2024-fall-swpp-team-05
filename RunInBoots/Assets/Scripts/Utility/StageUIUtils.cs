using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageUIUtils {

    public static void PlaceHeartIcons(int _lifeCount, int InitLifeCount = 9)
    {
        GameObject heartContainerObject = GameObject.FindGameObjectWithTag("HeartIconContainer");
        Transform heartIconContainer = heartContainerObject?.transform;
        GameObject liveHeartIconPrefab = Resources.Load<GameObject>("StageUIObject/LiveHeartIcon");
        GameObject deadHeartIconPrefab = Resources.Load<GameObject>("StageUIObject/DeadHeartIcon");

        for (int i = 0; i < _lifeCount; i++)
        {
            GameObject liveHeart = PoolManager.Instance.Pool(liveHeartIconPrefab, Vector3.zero, Quaternion.identity, heartIconContainer);
            liveHeart.transform.SetParent(heartIconContainer, false);
        }

        for (int i = _lifeCount; i < InitLifeCount; i++)
        {
            GameObject deadHeart = PoolManager.Instance.Pool(deadHeartIconPrefab, Vector3.zero, Quaternion.identity, heartIconContainer);
            deadHeart.transform.SetParent(heartIconContainer, false);
        }
    }

    public static void PlaceCatnipIcons(int totalCatnipCount, List<bool> isCatnipCollected, ref List<GameObject> catnipIcons)
    {
        catnipIcons.Clear();
        GameObject catnipIconPrefab = Resources.Load<GameObject>("StageUIObject/CatnipIcon");
        GameObject catnipContainerObject = GameObject.FindGameObjectWithTag("CatnipIconContainer");
        Transform catnipIconContainer = catnipContainerObject?.transform;
        for (int i = 0; i < totalCatnipCount; i++)
        {
            GameObject icon = PoolManager.Instance.Pool(catnipIconPrefab, Vector3.zero, Quaternion.identity, catnipIconContainer);
            icon.transform.SetParent(catnipIconContainer, false);
            catnipIcons.Add(icon);
            SetCatnipIconState(icon, isCatnipCollected[i]);
        }
    }

    public static void SetCatnipIconState(GameObject icon, bool isActive)
    {
        if (icon == null)
        {
            Debug.LogError("CatnipIcon is NULL");
            return;
        }
        Color iconColor = icon.GetComponent<UnityEngine.UI.Image>().color;
        iconColor.a = isActive ? 1.0f : 0.3f;
        icon.GetComponent<UnityEngine.UI.Image>().color = iconColor;
    }
}
