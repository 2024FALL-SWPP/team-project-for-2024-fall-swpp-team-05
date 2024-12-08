using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpawnUI : MonoBehaviour
{
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI lifeText;

    public void UpdateStageText(int stage)
    {
        stageText.text = "Stage: " + stage;
    }

    public void UpdateLifeText(int life)
    {
        lifeText.text = "Life: " + life;
    }
}
