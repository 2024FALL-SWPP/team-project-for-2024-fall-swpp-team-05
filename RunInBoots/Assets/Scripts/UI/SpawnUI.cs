using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpawnUI : MonoBehaviour
{
    public TextMeshProUGUI stageText;
    public GameObject lifeContainer;

    public void UpdateStageText(int stage)
    {
        stageText.text = "Stage: " + stage;
    }

    public void UpdateLifeContainer(int lifeCount)
    {
        Transform heartIconContainer = lifeContainer.transform;
        GameObject liveHeartIconPrefab = Resources.Load<GameObject>("StageUIObject/SpawnHeartIcon");

        for (int i = 0; i < lifeCount; i++)
        {
            GameObject liveHeart = PoolManager.Instance.Pool(liveHeartIconPrefab, Vector3.zero, Quaternion.identity, heartIconContainer);
            liveHeart.transform.SetParent(heartIconContainer, false);
        }
    }
}
