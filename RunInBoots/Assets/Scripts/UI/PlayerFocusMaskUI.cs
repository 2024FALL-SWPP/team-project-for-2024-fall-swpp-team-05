using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFocusMaskUI : MonoBehaviour
{
    private void Start()
    {
        //temporary fix for player not being spawned yet
        Invoke("SetToPlayerPosition", 0.5f);
    }


    public void SetToPlayerPosition()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        // set anchored position to player's screen position
        var playerScreenPos = Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up);
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = playerScreenPos- new Vector3(Screen.width/2f,Screen.height/2f);
    }
}
